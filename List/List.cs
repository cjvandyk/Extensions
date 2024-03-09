/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Extensions
{
    /// <summary>
    /// Extension methods for the System.Collections.Generic.List class.
    /// </summary>
    [Serializable]
    public static partial class ListExtensions
    {
        #region Sum<TimeSpan>
        /// <summary>
        /// Calculate the Sum of TimeSpans in the given List.
        /// </summary>
        /// <param name="lst">The given list of TimeSpans to calculate.</param>
        /// <param name="type">The type of return required as defined in the
        /// Contants.TimeSpanSumType enum.</param>
        /// <returns>The Sum of all the TimeSpans in the list, in the
        /// requested format.</returns>
        public static double Sum(this List<TimeSpan> lst, Constants.TimeSpanType type)
        {
            double result = 0;
            foreach (TimeSpan ts in lst)
            {
                switch (type)
                {
                    case Constants.TimeSpanType.Seconds:
                        result += ts.TotalSeconds;
                        break;
                    case Constants.TimeSpanType.Minutes:
                        result += (ts.TotalSeconds / 60);
                        break;
                    case Constants.TimeSpanType.Hours:
                        result += (ts.TotalSeconds / 60 / 60);
                        break;
                    case Constants.TimeSpanType.Days:
                        result += (ts.TotalSeconds / 60 / 60 / 24);
                        break;
                    case Constants.TimeSpanType.Weeks:
                        result += (ts.TotalSeconds / 60 / 60 / 24 / 7);
                        break;
                    case Constants.TimeSpanType.Years:
                        result += (ts.TotalSeconds / 60 / 60 / 24 / 365.25);
                        break;
                    case Constants.TimeSpanType.Decades:
                        result += (ts.TotalSeconds / 60 / 60 / 24 / 365.25 / 10);
                        break;
                    case Constants.TimeSpanType.Centuries:
                        result += (ts.TotalSeconds / 60 / 60 / 24 / 365.25 / 100);
                        break;
                    case Constants.TimeSpanType.Mellinnia:
                        result += (ts.TotalSeconds / 60 / 60 / 24 / 365.25 / 1000);
                        break;
                }
            }
            return result;
        }
        #endregion Sum<TimeSpan>

        #region Threading
        /// <summary>
        /// Method to process current list of type T objects in either
        /// multi-threaded or single-threaded fashion using the 
        /// processLoopInstanceMethod delegate method.  It also provides
        /// potential custom exception handling through the
        /// processLoopInstanceExceptionHandlerMethod delegate method.
        /// </summary>
        /// <typeparam name="T">The type of object being processed.</typeparam>
        /// <param name="lst">The current list of objects to process.</param>
        /// <param name="processLoopInstanceMethod">The delegate method that
        /// is called for every instance in the list.</param>
        /// <param name="multiThreaded">A boolean switch to force 
        /// multi-threaded processing of the list.</param>
        /// <param name="processLoopInstanceExceptionHandlerMethod">The
        /// delegate method that would be called to do custom exception
        /// processing.</param>
        /// <returns>A list of InstanceExceptionInfo objects representing
        /// exceptions captured during processing of the list.</returns>
        public static List<InstanceExceptionInfo> Process<T>(
            this List<T> lst,
            Action<T> processLoopInstanceMethod,
            bool multiThreaded = false,
            Action<Exception> processLoopInstanceExceptionHandlerMethod = null)
        {            
            if (multiThreaded)
            {
                return lst.MultiThread<T>(
                    processLoopInstanceMethod,
                    processLoopInstanceExceptionHandlerMethod);
            }
            else
            {
                return lst.SingleThread<T>(
                    processLoopInstanceMethod,
                    processLoopInstanceExceptionHandlerMethod);
            }
        }

        /// <summary>
        /// Process a list of objects of type T using multiple threads.
        /// </summary>
        /// <typeparam name="T">The type of objects in the list.</typeparam>
        /// <param name="lst">The list of objects to process.</param>
        /// <param name="processLoopInstanceMethod">The delegate method to 
        /// process each individual item.</param>
        /// <param name="processLoopInstanceExceptionHandlerMethod">The
        /// delegate method to call for custom exception handling.</param>
        /// <returns>A list of InstanceExceptionInfo objects containing
        /// details about all exceptions captured while processing the
        /// list.</returns>
        public static List<InstanceExceptionInfo> MultiThread<T>(
            this List<T> lst,
            Action<T> processLoopInstanceMethod,
            Action<Exception> processLoopInstanceExceptionHandlerMethod = null)
        {
            List<InstanceExceptionInfo> result = new List<InstanceExceptionInfo>();
            Parallel.ForEach(lst, obj =>
            {
                try
                {
                    processLoopInstanceMethod(obj);
                }
                catch (Exception ex)
                {
                    InstanceExceptionInfo current = new InstanceExceptionInfo();
                    current.Key = obj.GetHashCode().ToString();
                    current.Exceptions.Add(ex);
                    current.BinaryData.Add(current.Key, obj);
                    current.MultiThreaded = true;
                    lock (result)
                    {
                        result.Add(current);
                    }
                    if (processLoopInstanceExceptionHandlerMethod != null)
                    {
                        processLoopInstanceExceptionHandlerMethod(ex);
                    }
                }
            });
            return result;
        }

        /// <summary>
        /// Process a list of T items specifically in a single thread.  This
        /// is helpful in debugging scenarios where multi threading can wreck
        /// havoc on the ability to follow the code execution path.
        /// </summary>
        /// <typeparam name="T">The type of objects in the list.</typeparam>
        /// <param name="lst">The list of objects to process.</param>
        /// <param name="processLoopInstanceMethod">The method to process
        /// each individual item.</param>
        /// <param name="processLoopInstanceExceptionHandlerMethod">The method
        /// used in case of Exception to process said Exception.</param>
        public static List<InstanceExceptionInfo> SingleThread<T>(
            this List<T> lst,
            Action<T> processLoopInstanceMethod,
            Action<Exception> processLoopInstanceExceptionHandlerMethod = null)
        {
            List<InstanceExceptionInfo > result = new List<InstanceExceptionInfo>();
            foreach (T obj in lst)
            {
                try
                {
                    processLoopInstanceMethod(obj);
                }
                catch (Exception ex)
                {
                    InstanceExceptionInfo current = new InstanceExceptionInfo();
                    current.Key = obj.GetHashCode().ToString();
                    current.Exceptions.Add(ex);
                    current.BinaryData.Add(current.Key, obj);
                    current.MultiThreaded = false;
                    lock (result)
                    {
                        result.Add(current);
                    }
                    if (processLoopInstanceExceptionHandlerMethod != null)
                    {  
                        processLoopInstanceExceptionHandlerMethod(ex); 
                    }
                }
            }
            return result;
        }
        #endregion Threading

        #region TakeAndRemove()
        /// <summary>
        /// A method to take the requested number of items (or all items if
        /// the requested number is >= lst.Count()) and return the remainder.
        /// </summary>
        /// <typeparam name="T">The type of objects in the list.</typeparam>
        /// <param name="lst">The list of objects to process.</param>
        /// <param name="count">The number of return items to take from the
        /// head of the list before removing said items from the current
        /// list.</param>
        /// <returns>A list of type T objects containing the requested number
        /// of items from the head of the current list.</returns>
        public static List<T> TakeAndRemove<T>(
            this List<T> lst,
            int count)
        {
            //If the list contains fewer items than requested.
            if (lst.Count < count)
            {
                //Make a copy of the list.
                List<T> lstCopy = lst.ToList();
                //Reset the current list object.
                lst.Clear();
                //Return the copy.
                return lstCopy;
            }
            //Create the target list to contain the requested items.
            List<T> result = new List<T>();
            //Get each item.
            for (int C = 0; C < count; C++)
            { 
                //Add the first item of the source list to the target list. 
                result.Add(lst[0]);
                //Remove that item from the source list.
                lst.RemoveAt(0);
            }
            //Return the target list.
            return result;
        }
        #endregion TakeAndRemove()

        #region TryAdd()
        /// <summary>
        /// Checks if a given key is in the list.  If it isn't, it will
        /// attempt to add it.  If the addition fails, or the list is
        /// null it will return false.  If the addition succeeds or if the key
        /// already exist in the list, it will return true.
        /// </summary>
        /// <param name="lst">The list to which to add the key.</param>
        /// <param name="key">The key to add to the list.</param>
        /// <returns>True if the key was successfully added or already exist
        /// in the list, else on any failure it returns false.</returns>
        public static bool TryAdd(
            this List<string> lst,
            string key)
        {
            try
            {
                if (!lst.Contains(key))
                {
                    lst.Add(key);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion TryAdd()
    }
}
