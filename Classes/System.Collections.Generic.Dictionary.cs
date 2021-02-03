#pragma warning disable IDE1006, IDE0017, CS0162, IDE0060, IDE0079 // Naming Styles, Simplify declaration (FQCN used), break after return, Remove unused (string[] args, Remove unnecessary suppression)

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Quix/blob/master/LICENSE
/// </summary>

using System;

namespace Extensions
{
    public static class Dictionary
    {
        /// <summary>
        /// Convert given Dictionary<string, string> into a querystring.
        /// </summary>
        /// <param name="dic">The Dictionary containing the source keys and
        /// values</param>
        /// <param name="separator">The separator character to use.  
        /// Defaults to '&'.</param>
        /// <param name="assigner">The assigner characters to use.
        /// Defaults to '='.</param>
        /// <returns>The constructed querystring ready for use.</returns>
        public static string ToQueryString(
            this System.Collections.Generic.Dictionary<string, string> dic,
            char separator = '&',
            char assigner = '=')
        {
            string result = "?";
            foreach (string key in dic.Keys)
            {
                result += (key + assigner + dic[key] + separator);
            }
            return result.TrimEnd(separator);
        }
    }
}
