/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Extensions
{
    /// <summary>
    /// A class to provide a set of methods for persisting existing objects
    /// from memory to disk i.e. persist state.
    /// </summary>
    [Serializable]
    public static partial class State
    {
        /// <summary>
        /// The lock object to manage file update concurrency.
        /// </summary>
        private static readonly object _lock = new object();

        #region Helper Methods
        /// <summary>
        /// Method for calculating the target file path.
        /// </summary>
        /// <param name="listName">The list to be saved.  NOTE: The same
        /// name need to be used to later reload the list.</param>
        /// <param name="local">Save state file in the current folder.</param>
        /// <returns>A string of the full UNC path of the target.</returns>
        internal static string GetFileTarget(
            string listName,
            bool local = false)
        { 
            if (local)
            {
                return $"{listName}.state";
            }
            string path = Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments).TrimEnd('\\') +
                @"\Extensions.State";
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            return $"{path}\\{listName}.state";
        }
        #endregion Helper Methods

        #region Delegates
        /// <summary>
        /// A delegate method for loading state.
        /// </summary>
        /// <param name="loadStateMethod">The name of the worker method that
        /// loads the state.</param>
        public static void LoadState(Action loadStateMethod)
        {
            loadStateMethod();
        }

        /// <summary>
        /// A delegate method for saving state.
        /// </summary>
        /// <param name="saveStateMethod">The name of the worker method that
        /// loads the state.</param>
        public static void SaveState(Action saveStateMethod)
        {
            saveStateMethod();
        }
        #endregion Delegates

        #region Load
        /// <summary>
        /// The basic method to load a list from disk.  It only requires one
        /// parameter which is the name of the list to load.
        /// </summary>
        /// <param name="listName">The name of the list to load.  NOTE: This
        /// is the same name that was specified when the list was saved.</param>
        /// <param name="maxCacheAgeInDays">The maximum age in days the state
        /// file can be.  Default is 7 i.e. a week.</param>
        /// <returns>If the state file is newer than the current date/time 
        /// minus maxCacheAgeInDays, the reloaded list is returned.
        /// NOTE: The list is loaded as a typeof(object) which means you'll 
        /// need to case the return value to the original type to be 
        /// usable.</returns>
        public static object LoadStateList(string listName,
                                           int maxCacheAgeInDays = 7)
        {
            return LoadStateList(listName, typeof(object), false, maxCacheAgeInDays);
        }

        /// <summary>
        /// Method to load a list from disk.  It requires all three parameters
        /// in order to load the list.
        /// </summary>
        /// <param name="listName">The name of the list to load.  NOTE: This
        /// is the same name that was specified when the list was saved.</param>
        /// <param name="objectType">The type to which the loaded JSON will be
        /// cast, usually something like a List of a custom Class.</param>
        /// <param name="local">Is the state file in the execution folder.</param>
        /// <param name="maxCacheAgeInDays"></param>
        /// <returns>If the state file is newer than the current date/time plus
        /// the maxCacheAgeInDays, the reloaded list is returned, cast as the 
        /// specified object type.</returns>
        public static object LoadStateList(string listName,
                                           Type objectType,
                                           bool local = false,
                                           int maxCacheAgeInDays = 7)
        {
            string str = "";
            //Get the target filename for loading the list.
            string target = GetFileTarget(listName, local);
            //Lock the target
            lock (_lock)
            {
                //Check if the target file exists.
                if (System.IO.File.Exists(target))
                {
                    //State file exist now check age.
                    if ((maxCacheAgeInDays > 0) &&
                        (DateTime.UtcNow < System.IO.File.GetLastWriteTimeUtc(
                            target).AddDays(maxCacheAgeInDays)))
                    {
                        //State file is valid now load data.
                        str = (string)((object)str).Load(target);
                        //Check if something went wrong in the load process.
                        if (str != null)
                        {
                            return JsonSerializer.Deserialize(str, objectType);
                        }
                    }
                }
            }
            //All prerequisites were not met so return an empty list of the
            //given objectType.
            return (System.Collections.IList)Activator.CreateInstance(objectType);
        }

        /// <summary>
        /// Method to load a very large list of Graph ListItems from disk.  It
        /// requires all three parameters in order to load the list.  In order 
        /// to minimize RAM usage, the list is loaded directly to the eventual
        /// target container list thus eliminating in-memory duplication of
        /// the large list.
        /// </summary>
        /// <param name="listName">The name of the list to load.  NOTE: This is
        /// the same name that was specified when the list was saved.</param>
        /// <param name="targetList">A reference to the target list that will
        /// contain all the data.</param>
        /// <param name="maxCacheAgeInDays">The maximum age in days the state
        /// file can be.</param>
        /// <returns>If the state file is newer than the current date/time
        /// minus maxCacheAgeInDays, the cache file is loaded to the referenced
        /// list.</returns>
        public static string LoadStateListToRef(
            string listName,
            ref List<ListItem> targetList,
            int maxCacheAgeInDays = 7)
        {
            try
            {
                string str = null;
                //Get the target filename for loading the list.
                string target = GetFileTarget(listName);
                //Lock the target.
                lock (_lock)
                {
                    //Check if the target file exists.
                    if (System.IO.File.Exists(target))
                    {
                        //State file exist now check age.
                        if ((maxCacheAgeInDays > 0) &&
                            (DateTime.UtcNow < 
                            System.IO.File.GetLastWriteTimeUtc(target)
                                .AddDays(maxCacheAgeInDays)))
                        {
                            str = (string)((object)str).Load(target);
                        }
                    }
                }
                //Check if something went wrong in the load process.
                if (str != null)
                {
                    //Load was successful now reconstruct the list.
                    targetList = (List<ListItem>)
                        JsonSerializer.Deserialize(
                            str,
                            typeof(List<ListItem>));
                    return "OK";
                }
                //All prerequisites were not met so return null;
                targetList = null;
                return null;
            }
            catch (Exception ex)
            {
                targetList = null;
                return ex.ToString();
            }
        }
        #endregion Load

        #region Save
        /// <summary>
        /// Save a given list state to disk.
        /// </summary>
        /// <param name="listName">The name of the list to use in the Save
        /// operation.  The same name will be needed to reload the object
        /// state later.</param>
        /// <param name="list">The object to be saved.  NOTE: The object must
        /// be defined as [Serializable].</param>
        public static void SaveStateList(string listName,
                                         object list)
        {
            string str = "";
            //Get the target filename for saving the object.
            string target = GetFileTarget(listName);
            //Serialize the object string.
            str = JsonSerializer.Serialize(list);
            //Lock the target.
            lock (_lock)
            {
                ((object)str).Save(target);
            }
        }

        /// <summary>
        /// Save a given referenced list state to disk.
        /// </summary>
        /// <param name="listName">The name of the list to use in the Save
        /// operation.  The same name will be needed to reload the object
        /// state later.</param>
        /// <param name="list">The object to be saved.  NOTE: The object must
        /// be defined as [Serializable].</param>
        public static void SaveStateListFromRef(string listName,
                                                ref object list)
        {
            string str = "";
            //Get the target filename for saving the object.
            string target = GetFileTarget(listName);
            //Serialize the object string.
            str = JsonSerializer.Serialize(list);
            //Lock the target.
            lock (_lock)
            {
                ((object)str).Save(target);
            }
        }
        #endregion Save
    }
}
