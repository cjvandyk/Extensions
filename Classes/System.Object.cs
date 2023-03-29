#pragma warning disable CS1587, CS1998, IDE0059, IDE0028

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Collections.Generic;
using static Extensions.Core;

namespace Extensions
{
    /// <summary>
    /// Extension class to add Extension Properties to any class.
    /// </summary>
    [Serializable]
    public static partial class ObjectExtensions
    {
        /// <summary>
        /// The private dictionary object where extension properties are stored.
        /// </summary>
        private static Dictionary<string, object> extensionProperties = 
            new Dictionary<string, object>();
        /// <summary>
        /// The private object used to manage locks on file I/O.
        /// </summary>
        private static readonly object lockManager = new object();

        #region T Load<T>()
        /// <summary>
        /// Universal object method used to serialize ANY object from disk.
        /// </summary>
        /// <typeparam name="T">The type of the target object.</typeparam>
        /// <param name="obj">The triggering object.</param>
        /// <param name="filePath">The path on disk for the save file.</param>
        /// <returns>The object of type T loaded from disk.</returns>
        public static T Load<T>(this T obj,
                                   string filePath = "File.bin")
        {
            try
            {
                lock (lockManager)
                {
                    using (System.IO.Stream stream =
                        System.IO.File.Open(
                            filePath,
                            System.IO.FileMode.Open))
                    {
                        var serializer = new System.Runtime.Serialization.DataContractSerializer(typeof(T));
                        obj = (T)serializer.ReadObject(stream);
                        return obj;
                    }
                }
            }
            catch (Exception ex)
            {
                //Dump error.
                ConsoleColor currentColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ForegroundColor = currentColor;
                return default;
            }
        }
        #endregion T Load<T>()

        #region T Save<T>()
        /// <summary>
        /// Universal object method used to serialize ANY object to disk.
        /// </summary>
        /// <typeparam name="T">The type of the target object.</typeparam>
        /// <param name="obj">The triggering object.</param>
        /// <param name="filePath">The path on disk for the save file.</param>
        /// <returns>True if save successful, otherwise False.</returns>
        public static bool Save<T>(this T obj,
                                   string filePath = "File.bin")
        {
            try
            {
                lock (lockManager)
                {
                    using (System.IO.Stream stream =
                        System.IO.File.Open(
                            filePath,
                            System.IO.FileMode.Create))
                    {
                        var serializer = new System.Runtime.Serialization.DataContractSerializer(typeof(T));
                        serializer.WriteObject(stream, obj);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //Dump error.
                ConsoleColor currentColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ForegroundColor = currentColor;
                return false;
            }
        }
        #endregion T Save<T>()

        #region Get()
        /// <summary>
        /// Get the value of an extension property in the dictionary.
        /// </summary>
        /// <param name="obj">Our binding class.</param>
        /// <param name="key">The key for the value to be returned.
        /// This value is usually the name of the extension property
        /// who's value is being returned.</param>
        /// <returns>An object representing the value of the extension
        /// property.  You would have to manually cast the return value
        /// to the proper data type you know it to be.</returns>
        public static object Get(this object obj, 
                                 string key)
        {
            ValidateNoNulls(obj, key);
            return extensionProperties[key];
        }
        #endregion Get()

        #region Set()
        /// <summary>
        /// Set the value of an extension property in the dictionary.
        /// </summary>
        /// <param name="obj">Our binding class.</param>
        /// <param name="key">The key for the value.  This value is usually
        /// the name of the extension property being stored.</param>
        /// <param name="val">The value to which to set the extension
        /// property.</param>
        public static void Set(this object obj, 
                               string key, 
                               object val)
        {
            ValidateNoNulls(obj, key, val);
            extensionProperties[key] = val;
        }
        #endregion Set()
    }
}
#pragma warning restore CS1587, CS1998, IDE0059, IDE0028
