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
    public static class Array
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
        public static byte[] CopyTo(this byte[] bytes, int length, int start = 0)
        {
            byte[] result = new byte[length];
            for (int C = start; C < length + start; C++)
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
            string str = "";
            foreach (byte b in bytes)
            {
                str += (b.ToString() + 
                       (b < 10 ? "   " : 
                       (b < 100 ? "  " : " ")));
            }
            Console.WriteLine(str);
        }

        #endregion byte[]
    }
}
