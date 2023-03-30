//#pragma warning disable IDE1006, IDE0017, CS0162, IDE0060 // Naming Styles, Simplify declaration (FQCN used), break after return, Remove unused (string[] args)

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
    public static class Array
    {
        public static void Test()
        {
            //Test System.Array<byte>.CopyTo(),
            //     System.Array<byte>.Print().
            printf("********* Array Testing *********", ConsoleColor.Green);
            byte[] b1 = System.Text.Encoding.UTF8.GetBytes("blog.cjvandyk.com rocks!");
            b1.Print();
            byte[] b2 = b1.CopyTo(10);
            b2.Print();
            byte[] b3 = b1.CopyTo(10, 5);
            b3.Print();
        }
    }
}
