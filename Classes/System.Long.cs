/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Extensions.Universal;

namespace Extensions
{
    public static class LongExtensions
    {
        #region Method1
        public static bool IsEven(this long number)
        {
            ValidateNoNulls(number);
            return (number % 2 == 0);
        }

        public static bool IsOdd(this long number)
        {
            ValidateNoNulls(number);
            return (number % 2 != 0);
        }

        private static bool IsPrime(this long number)
        {
            //ValidateNoNulls(number);
            //if (number == 2) return true;
            //if (number.IsEven()) return false;
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

        public static long GetNthPrime(this long number, long Nth)
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
                if (current.IsPrime())
                {
                    lst.Add(current);
                    //lst.Save("primes.state");
                    //for (int C = 1; C < lst.Count; C++)
                    //{
                    //    if (lst[C] <= lst[C - 1])
                    //    {
                    //        int x = 0;
                    //    }
                    //}
                }
            }
            lst.Save("primes.state");
            return lst[(int)Nth - 1];
            ////System.Numerics.BigInteger
            //long totalPrimes = 0;
            //long sizeFactor = 2;
            //long s = Nth * sizeFactor;
            //bool[] primes = new bool[s];
            //while (totalPrimes < Nth)
            //{
            //    primes = GetPrimes(s);
            //    totalPrimes = primes.Where(C => C == true).Count();
            //    sizeFactor++;
            //    s = Nth * sizeFactor;
            //}
            //long NthPrime = CountPrimes(primes, Nth);
            //return NthPrime;
        }

        //public static long CountPrimes(bool[] primes, long Nth)
        //{
        //    long count = 0;
        //    for (long C = 2; C < primes.Count(); C++)
        //    {
        //        if (primes[C] == true)
        //        {
        //            count++;
        //            if (count == Nth)
        //            {
        //                return C;
        //            }
        //        }
        //    }
        //    return 0;
        //}

        //public static bool[] GetPrimes(long s)
        //{
        //    bool[] primes = Enumerable.Repeat(true, (int)s).ToArray();
        //    for (long C = 2; C < s; C++)
        //    {
        //        if (primes[C])
        //        {
        //            for (long v = C; v < s; v++)
        //            {
        //                if ((C * v) < s)
        //                {
        //                    primes[C * v] = false;
        //                }
        //                else
        //                {
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    return primes;
        //}
        #endregion Method1
    }
}
