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
    /// Extension methods for the System.Collections.Generic.List class.
    /// </summary>
    public static partial class ListExtensions
    {
        #region TryAdd()
        /// <summary>
        /// Checks if a given Type value is in the list.  If it isn't, it will
        /// attempt to add it.  If the addition fails, or the list is
        /// null it will return false.  If the addition succeeds it will return
        /// true.
        /// </summary>
        /// <param name="lst">The list to which to add the value.</param>
        /// <param name="val">The value to use in the addition.</param>
        /// <returns>True if the value was successfully added or already exist
        /// in the list, else on any failure it returns false.</returns>
        public static bool TryAdd<T>(
            this List<T> lst,
            T val)
        {
            try
            {
                if (!lst.Contains(val))
                {
                    lst.Add(val);
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
