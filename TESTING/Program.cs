#pragma warning disable IDE1006, IDE0017, CS0162, IDE0060 // Naming Styles, Simplify declaration (FQCN used), break after return, Remove unused (string[] args)

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;

using Extensions;

namespace TESTING
{
    class Program
    {
        static void Main(string[] args)
        {
            //If a number n is not a prime, it can be factored into two factors a and b:
            //n = a * b
            //Now a and b can't be both greater than the square root of n, since then the 
            //product a * b would be greater than sqrt(n) * sqrt(n) = n. So in any factorization 
            //of n, at least one of the factors must be smaller than the square root of n, and if 
            //we can't find any factors less than or equal to the square root, n must be a prime.
            //System.Numerics.BigInteger big = 0;
            //UInt64 big = (UInt64)Math.Pow(2, 82589933);
            //for (long C = 0; C < 82589933; C++)
            //{                
            //    big = big * 2;
            //}
            //long l = 3;
            //Console.WriteLine($"{l} {l.IsEven()}");
            //Console.WriteLine($"{l} {l.IsOdd()}");
            //l = 6;
            //Console.WriteLine($"{l} {l.IsEven()}");
            //Console.WriteLine($"{l} {l.IsOdd()}");
            ////553:17 vs 17:10
            //Console.WriteLine(l.GetNthPrimeAsync(10000000));
            Array.Test();
            DateTime.Test();
            Dictionary.Test();
            Double.Test();
            Object.Test();
            Process.Test();
            String.Test();
            TimeZoneInfo.Test();
            //WebException.Test();
        }
    }
}
