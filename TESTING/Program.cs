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
            long l = 3;
            Console.WriteLine($"{l} {l.IsEven()}");
            Console.WriteLine($"{l} {l.IsOdd()}");
            l = 6;
            Console.WriteLine($"{l} {l.IsEven()}");
            Console.WriteLine($"{l} {l.IsOdd()}");
            Console.WriteLine(l.GetNthPrime(1000000));
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
