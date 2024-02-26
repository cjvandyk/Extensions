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

//using Extensions;
//using Microsoft.Graph;
//using static Extensions.Core;
//using static System.Logit;

namespace TESTING
{
    class Program
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, CallingConvention = System.Runtime.InteropServices.CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        static void Main(string[] args)
        {
            Console.WriteLine(".");

            //Inf("Testing");

            ////Call the imported function with the cursor's current position
            //uint X = (uint)System.Windows.Forms.Cursor.Position.X;
            //uint Y = (uint)System.Windows.Forms.Cursor.Position.Y;
            //Random random = new Random(27);
            //for (int C = 0; C < 10000; C++)
            //{
            //    System.Threading.Thread.Sleep(random.Next(2000, 5000));
            //    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 80, 50, 0, 0);
            //}

            //var auth = Extensions.Identity.AuthMan.GetAuth("tenantid", "appid", "thumbprint", "tenantstring");
            ////bool[] b1 = new bool[] { true, false, false, true };
            ////bool[] b2 = new bool[] { true, false, false, true };
            ////Mersenne64.GreaterThan(ref b1, ref b2);
            ////Mersenne64 m = new Mersenne64(new bool[] { true, false, true, false, true });
            ////bool[] result = m.Mod(new bool[] { true, true, true });
            //////If a number n is not a prime, it can be factored into two factors a and b:
            //////n = a * b
            //////Now a and b can't be both greater than the square root of n, since then the 
            //////product a * b would be greater than sqrt(n) * sqrt(n) = n. So in any factorization 
            //////of n, at least one of the factors must be smaller than the square root of n, and if 
            //////we can't find any factors less than or equal to the square root, n must be a prime.
            //////System.Numerics.BigInteger big = 0;
            //////UInt64 big = (UInt64)Math.Pow(2, 82589933);
            //////for (long C = 0; C < 82589933; C++)
            //////{                
            //////    big = big * 2;
            //////}
            ////long l = 2;
            //////Console.WriteLine($"{l} {l.IsEven()}");
            //////Console.WriteLine($"{l} {l.IsOdd()}");
            //////l = 6;
            //////Console.WriteLine($"{l} {l.IsEven()}");
            //////Console.WriteLine($"{l} {l.IsOdd()}");
            ////////553:17 vs 17:10

            ////System.Numerics.BigInteger bint = new System.Numerics.BigInteger();
            ////for (System.Numerics.BigInteger C = 11; C < 1000000000; C++)
            ////{
            ////    bint = C * C;
            ////    if (((int)(bint.ToString().Length /2) + 0) < C.ToString().Length)
            ////    {
            ////        Console.WriteLine($"C:[{C.ToString().Length}] Value:[{bint.ToString().Length}]");
            ////    }
            ////}

            //////System.Numerics.BigInteger big = new System.Numerics.BigInteger();
            ////int big = 1234567890;
            ////Console.WriteLine(Convert.ToString(big, 2));
            ////Console.WriteLine(Mersenne32.BaseConverter.ToBinary(big));
            ////Console.WriteLine(Mersenne32.BaseConverter.Sqrt(big));

            ////System.DateTime startBitArray = System.DateTime.Now;
            ////bool[] bits = new bool[25000000];
            ////for (int C = 0; C < 25000000; C++)
            ////{
            ////    bits[C] = true;
            ////}
            ////System.DateTime stopBitArray = System.DateTime.Now;

            ////System.Numerics.BigInteger bigInteger;// = System.Numerics.BigInteger.Parse("1234567890");
            ////System.Numerics.BigInteger bigSquare;
            ////System.Text.StringBuilder bigString = new System.Text.StringBuilder();
            ////System.DateTime startString = System.DateTime.Now;
            ////for (int C = 1; C <= 1000000; C++)
            ////{
            ////    bigString.Append("1234567890");
            ////    //string s = String.Format("{0:n0}", bigString.Length)
            ////    //Console.WriteLine($"{System.String.Format("{0:n0}", bigString.Length)} digits");
            ////}
            ////System.DateTime stopString = System.DateTime.Now;
            ////bigInteger = System.Numerics.BigInteger.Parse(bigString.ToString());
            ////System.DateTime stopInt = System.DateTime.Now;
            ////bigSquare = Mersenne32.BaseConverter.Sqrt(bigInteger);
            ////System.DateTime stopSquare = System.DateTime.Now;
            ////Console.WriteLine($"Build string:{(stopString - startString)}");
            ////Console.WriteLine($"Create int:{(stopInt - stopString)}");
            ////Console.WriteLine($"Calc sqrt:{(stopSquare - stopInt)}");
            ////Console.WriteLine($"[{bigString.Length}][{bigSquare.ToString().Length}]");

            ////for (int C = 1; C <= 1000; C++)
            ////{
            ////    for (int repeat = 0; repeat < C; repeat++)
            ////    {
            ////        //bigString += "1234567890";
            ////    }
            ////    //bigInteger = System.Numerics.BigInteger.Parse(bigString);
            ////    //bigSquare = Mersenne32.BaseConverter.Sqrt(bigInteger);
            ////    Console.WriteLine($"[{bigString.Length}][{bigSquare.ToString().Length}]");
            ////}

            ////Mersenne32.BaseConverter.Convert(10, 2, "16");

            ////bool prime = false;
            ////System.UInt64 bi = 2;
            ////long evenPrime = 0;
            ////long oddPrime = 0;
            ////for (System.UInt64 C = 1; C < 1000; C++)
            ////{
            ////    prime = (bi - 1).IsPrime();
            ////    if (prime)
            ////    {
            ////        if ((C % 2) == 0)
            ////        {
            ////            evenPrime++;
            ////        }
            ////        else
            ////        {
            ////            oddPrime++;
            ////        }
            ////    }
            ////    Console.WriteLine($"{C}:{(bi - 1)}: {prime} (Even:{evenPrime} Odd:{oddPrime}");
            ////    if (C == 60)
            ////    {
            ////        Console.WriteLine("Here");
            ////    }
            ////    bi = bi * 2;
            ////}
            ////((System.Numerics.BigInteger)99).IsPrime();
            ////l.GetNthPrimeAsync(4227946);
            ////TimeSpan ts;
            ////System.DateTime start;
            ////start = System.DateTime.Now;
            ////Console.WriteLine(start);
            ////Console.WriteLine(l.GetNthPrimeAsync(10000));
            ////ts = System.DateTime.Now - start;
            ////Console.WriteLine(ts.TotalSeconds);
            ////start = System.DateTime.Now;
            ////Console.WriteLine(start);
            ////Console.WriteLine(l.GetNthPrime(10000));
            ////ts = System.DateTime.Now - start;
            ////Console.WriteLine(ts.TotalSeconds);
            ////start = System.DateTime.Now;
            ////Console.WriteLine(start);
            ////Console.WriteLine(l.GetNthPrimeAsync(100000));
            ////ts = System.DateTime.Now - start;
            ////Console.WriteLine(ts.TotalSeconds);
            ////start = System.DateTime.Now;
            ////Console.WriteLine(start);
            ////Console.WriteLine(l.GetNthPrime(100000));
            ////ts = System.DateTime.Now - start;
            ////Console.WriteLine(ts.TotalSeconds);
            //////Console.WriteLine("================================");
            //////Array.Test();
            //////DateTime.Test();
            //////Dictionary.Test();
            //////Double.Test();
            //////Object.Test();
            //////Process.Test();
            //////String.Test();
            //////TimeZoneInfo.Test();
            //////PrimeNumbers.Test();
            ////WebException.Test();

            ////Test System.String.Left()

        }
    }
}
