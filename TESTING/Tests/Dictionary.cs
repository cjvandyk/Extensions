/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>
using System;

using Extensions;
using static Extensions.Core;

namespace TESTING
{
    public static class Dictionary
    {
        public static void Test()
        {
            //Test System.Collections.Generic.Dictionary.ToQueryString()
            printf("********* Dictionary Testing *********", ConsoleColor.Green);
            System.Collections.Generic.Dictionary<string, string> dic1 = 
                new System.Collections.Generic.Dictionary<string, string>();
            dic1.Add("Parm1", "Val1");
            dic1.Add("Parm2", "Val2");
            dic1.Add("Parm3", "Val3");
            printf(dic1.ToQueryString());
        }
    }
}
