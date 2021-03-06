﻿/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Extensions;
using static Extensions.Universal;

namespace TemplateClassLibrary
{
    public static class Class1
    {
        #region Method1
        public static void Method1(string arg1, double arg2)
        {
            ValidateNoNulls(arg1, arg2);
        }
        #endregion Method1
    }
}
