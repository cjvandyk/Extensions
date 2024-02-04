/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Collections.Generic;
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
        public static double Sum(this List<TimeSpan> lst, Constants.TimeSpanSumType type)
        {
            double result = 0;
            foreach (TimeSpan ts in lst)
            {
                switch (type)
                {
                    case Constants.TimeSpanSumType.Seconds:
                        result += ts.TotalSeconds;
                        break;
                    case Constants.TimeSpanSumType.Minutes:
                        result += (ts.TotalSeconds / 60);
                        break;
                    case Constants.TimeSpanSumType.Hours:
                        result += (ts.TotalSeconds / 60 / 60);
                        break;
                    case Constants.TimeSpanSumType.Days:
                        result += (ts.TotalSeconds / 60 / 60 / 24);
                        break;
                    case Constants.TimeSpanSumType.Weeks:
                        result += (ts.TotalSeconds / 60 / 60 / 24 / 7);
                        break;
                    case Constants.TimeSpanSumType.Years:
                        result += (ts.TotalSeconds / 60 / 60 / 24 / 365.25);
                        break;
                    case Constants.TimeSpanSumType.Decades:
                        result += (ts.TotalSeconds / 60 / 60 / 24 / 365.25 / 10);
                        break;
                    case Constants.TimeSpanSumType.Centuries:
                        result += (ts.TotalSeconds / 60 / 60 / 24 / 365.25 / 100);
                        break;
                    case Constants.TimeSpanSumType.Mellinnia:
                        result += (ts.TotalSeconds / 60 / 60 / 24 / 365.25 / 1000);
                        break;
                }
            }
            return result;
        }
        #endregion Sum<TimeSpan>

        #region Threading
        /// <summary>
        /// Process a list of objects of type T using multiple threads.
        /// </summary>
        /// <typeparam name="T">The type of objects in the list.</typeparam>
        /// <param name="lst">The list of objects to process.</param>
        /// <param name="processLoopInstanceMethod">The method to process
        /// each individual item.</param>
        public static void MultiThread<T>(
            this List<T> lst,
            Action<T> processLoopInstanceMethod)
        {
            Parallel.ForEach(lst, obj =>
            {
                try
                {
                    processLoopInstanceMethod(obj);
                }
                catch (System.Net.WebException wex)
                {
                    Logit.Err(wex.ToString());
                }
                catch (Exception ex)
                {
                    Logit.Err(ex.ToString());
                }
            });
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
        public static void SingleThread<T>(
            this List<T> lst,
            Action<T> processLoopInstanceMethod)
        {
            foreach (T obj in lst)
            {
                try
                {
                    processLoopInstanceMethod(obj);
                }
                catch (System.Net.WebException wex)
                {
                    Logit.Err(wex.ToString());
                }
                catch (Exception ex)
                {
                    Logit.Err(ex.ToString());
                }
            }
        }
        #endregion Threading

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
