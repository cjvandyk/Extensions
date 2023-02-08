#pragma warning disable CS1587, CS1998, IDE0059, IDE0028

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Collections.Generic;
using static Extensions.Universal;

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
