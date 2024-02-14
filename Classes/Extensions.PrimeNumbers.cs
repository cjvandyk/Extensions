/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;

namespace Extensions
{
    /// <summary>
    /// Class to contain prime number details during async processing.
    /// </summary>
    [Serializable]
    public partial class PrimeNumber
    {
        /// <summary>
        /// The number being evaluated as a prime number.
        /// </summary>
        public System.UInt64 Number { get; set; }
        /// <summary>
        /// The result of the prime number evaluation.
        /// </summary>
        public bool IsPrime { get; set; } = false;

        /// <summary>
        /// Class constructor method.
        /// </summary>
        /// <param name="target">The target number to store.</param>
        public PrimeNumber(System.UInt64 target)
        {
            Number = target;
            IsPrime = false;
        }
    }
}
