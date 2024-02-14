/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;

namespace Extensions
{
    [Serializable]
    public static partial class CoreBase
    {
        public static string TenantString { get; set; } = "Contoso";

        public static void Initialize(string tenantString)
        {
            TenantString = tenantString;
        }
    }
}
