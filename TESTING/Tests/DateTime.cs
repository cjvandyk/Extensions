#pragma warning disable IDE1006, IDE0017, CS0162, IDE0060 // Naming Styles, Simplify declaration (FQCN used), break after return, Remove unused (string[] args)

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Quix/blob/master/LICENSE
/// </summary>
using System;

using Extensions;

namespace TESTING
{
    public static class DateTime
    {
        public static void Test()
        {
            //Test .ToTimeZone()
            System.DateTime now = System.DateTime.Now;
            Console.WriteLine(now);
            Console.WriteLine(
                now.ToTimeZone(
                    Constants.TimeZone.CentralStandardTime, 
                    Constants.TimeZone.EasternStandardTime));
        }
    }
}
