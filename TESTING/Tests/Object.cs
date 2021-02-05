﻿#pragma warning disable IDE1006, IDE0017, CS0162, IDE0060 // Naming Styles, Simplify declaration (FQCN used), break after return, Remove unused (string[] args)

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Quix/blob/master/LICENSE
/// </summary>

using System;

using Extensions;
using static Extensions.Universal;

namespace TESTING
{
    public static class Object
    {
        public static void Test()
        {
            //Test System.Object.Set() and 
            //     System.Object.Get()
            printf("********* Object Testing *********", ConsoleColor.Green);
            string s = "abc";
            printf(s);
            s.Set("s", (object)"xyz");
            printf(s);
            printf("s");
            printf(s);
            int i = 3;
            printf(i);
            i.Set("i", (object)9);
            printf(i);
            i = (int)i.Get("i");
            printf(i);
            printf(i.Get("s"));
            printf(s.Get("i"));
        }
    }
}
