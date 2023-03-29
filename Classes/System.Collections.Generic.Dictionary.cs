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
    /// Extension methods for the System.Collections.Generic.Dictionary class.
    /// </summary>
    [Serializable]
    public static partial class DictionaryExtensions
    {
        #region ToQueryString()
        /// <summary>
        /// Convert given Dictionary into a querystring.
        /// </summary>
        /// <param name="dic">The Dictionary containing the source keys and
        /// values</param>
        /// <param name="separator">The separator character to use.  
        /// Defaults to ampersand.</param>
        /// <param name="assigner">The assigner characters to use.
        /// Defaults to '='.</param>
        /// <returns>The constructed querystring ready for use.</returns>
        public static string ToQueryString(
            this Dictionary<string, string> dic,
            char separator = '&',
            char assigner = '=')
        {
            ValidateNoNulls(dic, separator, assigner);
            string result = "?";
            foreach (string key in dic.Keys)
            {
                result += (key + assigner + dic[key] + separator);
            }
            return result.TrimEnd(separator);
        }
        #endregion ToQueryString()
    }
}
#pragma warning restore CS1587, CS1998, IDE0059, IDE0028
