/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using static Extensions.Core;

namespace Extensions
{
    /// <summary>
    /// Extension methods for the System.Array class.
    /// </summary>
    [Serializable]
    public static partial class ArrayExtensions
    {
        #region byte[]
        /// <summary>
        /// Copies a given length of bytes from a byte[] starting at a 
        /// definable offset.
        /// </summary>
        /// <param name="bytes">The byte array source being copied from.</param>
        /// <param name="length">The number of bytes to copy.</param>
        /// <param name="start">The offset starting point to start the copy.</param>
        /// <returns>The copied bytes in a byte[].</returns>
        public static byte[] CopyTo(this byte[] bytes, 
                                    int length, 
                                    int start = 0)
        {
            Validate(Constants.ErrorTypeAll, bytes, length, start);
            if (length == 0)
            {
                throw new ArgumentOutOfRangeException(
                    "length",
                    length,
                    "Length of the byte[] cannot be zero.");
            }
            if (start > bytes.Length - 1)
            {
                throw new ArgumentOutOfRangeException(
                    "start",
                    start,
                    "Starting offset cannot be larger than the byte[] length");
            }
            byte[] result = new byte[length];
            for (int C = start; C < (length + start > bytes.Length ?
                                     bytes.Length - start :
                                     length + start); C++)
            {
                result[C - start] = bytes[C];
            }
            return result;
        }

        /// <summary>
        /// Print the byte[] to console, separated by spaces and space padded
        /// on the right to allow proper alignment for debug/testing output.
        /// </summary>
        /// <param name="bytes">The byte array to print to console.</param>
        public static void Print(this byte[] bytes)
        {
            ValidateNoNulls(bytes);
            string str = "";
            foreach (byte b in bytes)
            {
                str += (b.ToString() + 
                       (b < 10 ? "   " : 
                       (b < 100 ? "  " : " ")));
            }
            Core.Printf(str);
        }
        #endregion byte[]
    }
}
