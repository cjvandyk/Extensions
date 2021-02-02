using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
