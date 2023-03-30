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
using static Extensions.Core;

namespace TESTING
{
    public static class Guid
    {
        public static void Test()
        {
            //Test .NewCustomGuid()
            printf("********* Guid Testing *********", ConsoleColor.Green);
            System.Guid g = Extensions.Guid.NewCustomGuid("27c0e");
            printf(g.ToString());
        }
    }
}
