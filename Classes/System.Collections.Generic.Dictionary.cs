using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
