#pragma warning disable CS1587, CS0162, CS1998, IDE0059, IDE0028

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using static Extensions.Universal;

namespace Extensions
{
    /// <summary>
    /// Extension methods for the System.Text.StringBuilder
    /// classes.
    /// </summary>
    [Serializable]
    public static partial class StringBuilderExtensions
    {
        #region IndexOf()
        /// <summary>
        /// Returns the offset location of the index value.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="index"></param>
        /// <param name="startindex"></param>
        /// <param name="ignorecase"></param>
        /// <returns>The integer offset of the index string in the source
        /// StringBuilder object.</returns>
        public static int IndexOf(this System.Text.StringBuilder str,
                                  string index,
                                  int startindex = 0,
                                  bool ignorecase = true)
        {
            ValidateNoNulls(str, index);
            int counter;
            int targetlength = index.Length;
            if (ignorecase)
            {
                for (int C = startindex; C < ((str.Length - targetlength) + 1); C++)
                {
                    if (Char.ToLower(str[C]) == Char.ToLower(index[0]))
                    {
                        counter = 1;
                        while ((counter < targetlength) && 
                               (Char.ToLower(str[C + counter]) == Char.ToLower(index[counter])))
                        {
                            counter++;
                        }
                        if (counter == targetlength)
                        {
                            return C;
                        }
                    }
                }
                return -1;
            }
            else
            {
                for (int C = startindex; C < ((str.Length - targetlength) + 1); C++)
                {
                    if (str[C] == index[0])
                    {
                        counter = 1;
                        while ((counter < targetlength) && (str[C + counter] == index[counter]))
                        {
                            counter++;
                        }
                        if (counter == targetlength)
                        {
                            return C;
                        }
                    }
                }
            }
            return -1;
        }
        #endregion IndexOf()

    }
}
#pragma warning restore CS1587, CS0162, CS1998, IDE0059, IDE0028
