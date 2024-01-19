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
    /// Extensions for the System.UInt64 class.
    /// </summary>
    [Serializable]
    public static partial class UInt64Extensions
    {
        #region IsEven()
        /// <summary>
        /// Checks if the given number is even.
        /// </summary>
        /// <param name="number">The given number to check.</param>
        /// <returns>True if the number is even, else False.</returns>
        public static bool IsEven(this System.UInt64 number)
        {
            ValidateNoNulls(number);
            return (number % 2 == 0);
        }
        #endregion IsEven()

        #region IsOdd()
        /// <summary>
        /// Checks if the given number is odd.
        /// </summary>
        /// <param name="number">The given number to check.</param>
        /// <returns>True if the number is odd, else False.</returns>
        public static bool IsOdd(this System.UInt64 number)
        {
            ValidateNoNulls(number);
            return (number % 2 != 0);
        }
        #endregion IsOdd()

        #region IsPrime()
        /// <summary>
        /// For internal use only. Does not waste CPU cycles validating input
        /// data or checking for 2 or even numbers.  This method assumes it 
        /// will always be passed odd numbers greater than 2.
        /// Checks if the given number is a prime number.
        /// </summary>
        /// <param name="number">The given number to check.</param>
        /// <param name="INTERNAL_USE_ONLY">Method to distinguish from other overloads.</param>
        /// <returns>True if the given number is a prime number, else False.</returns>
        public static bool IsPrime(this System.UInt64 number,
                                   bool INTERNAL_USE_ONLY)
        {
            for (System.UInt64 C = 3; C <= (Math.Ceiling(Math.Sqrt(number))); C += 2)
            {
                if ((number % C) == 0)
                {
                    return false;
                    break;
                }
            }
            return true;
        }
        #endregion IsPrime()
    }
}
