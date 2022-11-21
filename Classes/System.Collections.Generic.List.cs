#pragma warning disable CS1587

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Collections.Generic;

using System.Threading.Tasks;
using System.Linq;
using Extensions;
using static Extensions.Universal;

namespace Extensions
{
    /// <summary>
    /// Extension methods for the System.Collections.Generic.List class.
    /// </summary>
    public static class ListExtensions
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
                processLoopInstanceMethod(obj);
            });
        }

        /// <summary>
        /// Process a list of objects of type T using a single thread.
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
                processLoopInstanceMethod(obj);
            }
        }
        #endregion Sum<TimeSpan>
    }
}
#pragma warning restore CS1587
