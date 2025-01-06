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
        #region Find()
        /// <summary>
        /// Find the key, given a value from the Dictionary.
        /// </summary>
        /// <typeparam name="T">Generic Type</typeparam>
        /// <param name="dic">The dictionary to search.</param>
        /// <param name="value">The value fo which to search.</param>
        /// <returns>The key of the KVP if the value exists, else null.</returns>
        public static T Find<T>(this Dictionary<T, T> dic, T value) where T : class 
        { 
            //Ensure our trigger dictionary is not null.
            if (dic != null)
            {
                //Scan all values in the dictionary for the given value.
                foreach (var item in dic)
                {
                    //If found, return the key.
                    if (item.Value == value)
                    {
                        return item.Key;
                    }
                }
            }
            return null;
        }
        #endregion

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

        #region TryAdd()
        /// <summary>
        /// Checks if a given KVP is in the dictionary.  If it isn't, it will
        /// attempt to add it.  If the addition fails, or the dictionary is
        /// null it will return false.  If the addition succeeds or if the key
        /// already exist in the dictionary, it will return true.
        /// </summary>
        /// <param name="dic">The dictionary to which to add the KVP.</param>
        /// <param name="key">The key to use in the addition.</param>
        /// <param name="val">The value to use in the addition.</param>
        /// <returns>True if the KVP was successfully added or already exist
        /// in the dictionary, else on any failure it returns false.</returns>
        public static bool TryAdd(
            this Dictionary<object, object> dic,
            object key,
            object val)
        {
            try
            {
                if (!dic.ContainsKey(key))
                {
                    dic.Add(key, val);
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
