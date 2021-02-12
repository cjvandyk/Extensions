/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Collections.Generic;

using Extensions;
using static Extensions.Universal;

namespace Extensions
{
    /// <summary>
    /// Extensions to the long class.
    /// </summary>
    public static class LongExtensions
    {
        #region IsEven()
        /// <summary>
        /// Checks if the given number is even.
        /// </summary>
        /// <param name="number">The given number to check.</param>
        /// <returns>True if the number is even, else False.</returns>
        public static bool IsEven(this long number)
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
        public static bool IsOdd(this long number)
        {
            ValidateNoNulls(number);
            return (number % 2 != 0);
        }
        #endregion IsOdd()

        #region PrimeNumbers
        /// <summary>
        /// Checks if the given number is a prime number.
        /// </summary>
        /// <param name="number">The given number to check.</param>
        /// <returns>True if the given number is a prime number, else False.</returns>
        private static bool IsPrime(this long number)
        {
            ValidateNoNulls(number);
            if (number == 2) return true;
            if (number.IsEven()) return false;
            bool prime = true;
            for (long C = 3; C < number; C+=2)
            {
                if ((number % C) == 0)
                {
                    prime = false;
                    break;
                }
            }
            return prime;
        }

        /// <summary>
        /// For internal use only. Does not waste CPU cycles validating input
        /// data or checking for 2 or even numbers.  This method assumes it 
        /// will always be passed odd numbers greater than 2.
        /// Checks if the given number is a prime number.
        /// </summary>
        /// <param name="number">The given number to check.</param>
        /// <returns>True if the given number is a prime number, else False.</returns>
        private static bool IsPrime(this long number, 
                                    bool priv = true)
        {
            bool prime = true;
            for (long C = 3; C < number; C += 2)
            {
                if ((number % C) == 0)
                {
                    prime = false;
                    break;
                }
            }
            return prime;
        }

        /// <summary>
        /// Gets the requested Nth prime number.
        /// </summary>
        /// <param name="number">The trigger variable.</param>
        /// <param name="Nth">The count number of the requested prime.</param>
        /// <returns>The requested Nth prime number.</returns>
        private static long GetNthPrime(this long number, long Nth)
        {
            List<long> lst = new List<long>();
            lst = lst.Load("primes.state");
            long current = 3;
            if (lst == null)
            {
                lst = new List<long>();
                lst.Add(2);
                lst.Add(3);
            }
            if (lst.Count >= Nth)
            {
                return lst[(int)Nth - 1];
            }
            else
            {
                current = lst[lst.Count - 1];
            }
            while (lst.Count < Nth)
            {
                current += 2;
                if (current.IsPrime(true))
                {
                    lst.Add(current);
                }
            }
            lst.Save("primes.state");
            return lst[(int)Nth - 1];
        }

        private static async System.Threading.Tasks.Task<long> GetNthPrimeAsync(this long number, long Nth)
        {
            List<long> lst = new List<long>();
            lst = lst.Load("primes.state");
            long current = 3;
            if (lst == null)
            {
                lst = new List<long>();
                lst.Add(2);
                lst.Add(3);
            }
            if (lst.Count >= Nth)
            {
                return lst[(int)Nth - 1];
            }
            else
            {
                current = lst[lst.Count - 1];
            }
            //List<System.Threading.Tasks.Task> tasks = new List<System.Threading.Tasks.Task>();
            int cpus = Environment.ProcessorCount - 1;
            while (lst.Count < Nth)
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
                if (task1.Result.isPrime)
                {
                    lst.Add(task1.Result.number);
                }
                if (task2.Result.isPrime)
                {
                    lst.Add(task2.Result.number);
                }
                if (task3.Result.isPrime)
                {
                    lst.Add(task3.Result.number);
                }
                if (task4.Result.isPrime)
                {
                    lst.Add(task4.Result.number);
                }
                if (task5.Result.isPrime)
                {
                    lst.Add(task5.Result.number);
                }
                if (task6.Result.isPrime)
                {
                    lst.Add(task6.Result.number);
                }
                if (task7.Result.isPrime)
                {
                    lst.Add(task7.Result.number);
                }
                //if (current.IsPrimeAsync(true))
                //{
                //    lst.Add(current);
                //}
            }
            lst.Save("primes.state");
            return lst[(int)Nth - 1];
        }

        private async static System.Threading.Tasks.Task<PrimeNumber> IsPrimeAsync(PrimeNumber prime)
        {
            //bool prime = true;
            for (long C = 3; C < prime.number; C += 2)
            {
                if ((prime.number % C) == 0)
                {
                    //prime = false;
                    break;
                }
            }
            prime.isPrime = true;
            return prime;
        }

        #endregion PrimeNumbers
    }
}
