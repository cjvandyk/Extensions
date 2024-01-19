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
    /// Extensions for the System.Int64 class.
    /// </summary>
    [Serializable]
    public static partial class Int64Extensions
    {
        #region PrimeNumbers
        /// <summary>
        /// String path where to .Save() and .Load() the prime number list
        /// so as to avoid recalculating the same numbers repeatedly.
        /// </summary>
        public static string PrimeStatePath = "primes.state";

        /// <summary>
        /// Gets the requested Nth prime number using asynchronous, parallel
        /// processing techniques.
        /// </summary>
        /// <param name="Nth">The count number of the requested prime.</param>
        /// <returns>The requested Nth prime number.</returns>
        public static async System.Threading.Tasks.Task<System.UInt64> GetNthPrimeAsync(System.UInt64 Nth)
        {
            List<System.UInt64> lst = new List<System.UInt64>();
            lst = lst.Load(PrimeStatePath);
            System.UInt64 current = 3;
            if (lst == null)
            {
                lst = new List<System.UInt64>();
                lst.Add(2);
                lst.Add(3);
            }
            if ((System.UInt64)lst.Count >= Nth)
            {
                return lst[(int)Nth - 1];
            }
            else
            {
                current = lst[lst.Count - 1];
            }
            //List<System.Threading.Tasks.Task> tasks = new List<System.Threading.Tasks.Task>();
            int cpus = Environment.ProcessorCount - 1;
            while ((System.UInt64)lst.Count < Nth)
            {
                //for (int C = 0; C < cpus; C++)
                //{
                //    current += 2;
                //    tasks.Add(IsPrimeAsync(new PrimeNumber(current)));
                //}
                current += 2;
                var task1 = IsPrimeAsync(new PrimeNumber(current));
                current += 2;
                var task2 = IsPrimeAsync(new PrimeNumber(current));
                current += 2;
                var task3 = IsPrimeAsync(new PrimeNumber(current));
                current += 2;
                var task4 = IsPrimeAsync(new PrimeNumber(current));
                current += 2;
                var task5 = IsPrimeAsync(new PrimeNumber(current));
                current += 2;
                var task6 = IsPrimeAsync(new PrimeNumber(current));
                current += 2;
                var task7 = IsPrimeAsync(new PrimeNumber(current));
                PrimeNumber[] primes = await System.Threading.Tasks.Task.WhenAll(task1, task2, task3, task4, task5, task6, task7);
                if (task1.Result.IsPrime)
                {
                    lst.Add(task1.Result.Number);
                }
                if (task2.Result.IsPrime)
                {
                    lst.Add(task2.Result.Number);
                }
                if (task3.Result.IsPrime)
                {
                    lst.Add(task3.Result.Number);
                }
                if (task4.Result.IsPrime)
                {
                    lst.Add(task4.Result.Number);
                }
                if (task5.Result.IsPrime)
                {
                    lst.Add(task5.Result.Number);
                }
                if (task6.Result.IsPrime)
                {
                    lst.Add(task6.Result.Number);
                }
                if (task7.Result.IsPrime)
                {
                    lst.Add(task7.Result.Number);
                }
                //if (current.IsPrimeAsync(true))
                //{
                //    lst.Add(current);
                //}
            }
            lst.Save(PrimeStatePath);
            return lst[(int)Nth - 1];
        }

        /// <summary>
        /// Internal async method to determin
        /// </summary>
        /// <param name="prime"></param>
        /// <returns></returns>
        private async static System.Threading.Tasks.Task<PrimeNumber> IsPrimeAsync(PrimeNumber prime)
        {
            for (ulong C = 3; C <= (Math.Ceiling(Math.Sqrt(prime.Number))); C += 2)
            {
                if ((prime.Number % C) == 0)
                {
                    return prime;
                }
            }
            prime.IsPrime = true;
            return prime;
        }

        /// <summary>
        /// Gets the requested Nth prime number.
        /// </summary>
        /// <param name="Nth">The count number of the requested prime.</param>
        /// <returns>The requested Nth prime number.</returns>
        public static System.UInt64 GetNthPrime(System.UInt64 Nth)
        {
            List<System.UInt64> lst = new List<System.UInt64>();
            lst = lst.Load(PrimeStatePath);
            System.UInt64 current = 3;
            if (lst == null)
            {
                lst = new List<System.UInt64>();
                lst.Add(2);
                lst.Add(3);
            }
            if ((System.UInt64)lst.Count >= Nth)
            {
                return lst[(int)Nth - 1];
            }
            else
            {
                current = lst[lst.Count - 1];
            }
            while ((System.UInt64)lst.Count < Nth)
            {
                current += 2;
                if (ULongExtensions.IsPrime((System.UInt64)current))
                {
                    lst.Add(current);
                }
            }
            lst.Save(PrimeStatePath);
            return lst[(int)Nth - 1];
        }

        /// <summary>
        /// Checks if the given number is a prime number.
        /// </summary>
        /// <param name="number">The given number to check.</param>
        /// <returns>True if the given number is a prime number, else False.</returns>
        private static bool IsPrime(this System.Numerics.BigInteger number)
        {
            Validate(Constants.ErrorTypeAll, number);
            if (number == 2)
                return true;
            if (IsEven(number))
                return false;
            bool prime = true;
            for (long C = 3; C <= Sqrt(number); C += 2) //(Math.Ceiling(Math.Sqrt(number))); C+=2)
            {
                if ((number % C) == 0)
                {
                    prime = false;
                    break;
                }
            }
            return prime;
        }
        #endregion PrimeNumbers

        private static System.Numerics.BigInteger Sqrt(
            this System.Numerics.BigInteger number)
        {
            System.Numerics.BigInteger n = 0, p = 0;
            if (number == System.Numerics.BigInteger.Zero)
                return System.Numerics.BigInteger.Zero;
            var high = number >> 1;
            var low = System.Numerics.BigInteger.Zero;
            while (high > low + 1)
            {
                n = (high + low) >> 1;
                p = n * n;
                if (number < p)
                    high = n;
                else if (number > p)
                    low = n;
                else
                    break;
            }
            return number == p ? n : low;
        }

        #region IsEven()
        /// <summary>
        /// Checks if the given number is even.
        /// </summary>
        /// <param name="number">The given number to check.</param>
        /// <returns>True if the number is even, else False.</returns>
        private static bool IsEven(this System.Numerics.BigInteger number)
        {
            Validate(Constants.ErrorTypeAll, number);
            return (number % 2 == 0);
        }
        #endregion IsEven()

        #region IsOdd()
        /// <summary>
        /// Checks if the given number is odd.
        /// </summary>
        /// <param name="number">The given number to check.</param>
        /// <returns>True if the number is odd, else False.</returns>
        private static bool IsOdd(this System.Numerics.BigInteger number)
        {
            Validate(Constants.ErrorTypeAll, number);
            return (number % 2 != 0);
        }
        #endregion IsOdd()

        #region IsEven()
        /// <summary>
        /// Checks if the given number is even.
        /// </summary>
        /// <param name="number">The given number to check.</param>
        /// <returns>True if the number is even, else False.</returns>
        public static bool IsEven(this System.Int64 number)
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
        public static bool IsOdd(this System.Int64 number)
        {
            ValidateNoNulls(number);
            return (number % 2 != 0);
        }
        #endregion IsOdd()

        #region IsPrime()
        /// <summary>
        /// Checks if the given number is a prime number.
        /// </summary>
        /// <param name="number">The given number to evaluate as a prime.</param>
        /// <returns>True if the given number is prime, else False.</returns>
        public static bool IsPrime(this System.Int64 number)
        {
            return UInt64Extensions.IsPrime((System.UInt64)number, true);
        }
        #endregion IsPrime()
    }
}
