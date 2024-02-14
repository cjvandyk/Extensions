using System;
/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>
namespace Extensions.Identity
{
    /// <summary>
    /// Auth class for holding authentication objects.
    /// </summary>
    public partial class Auth
    {
        /// <summary>
        /// The type of ClientApplication to use.
        /// </summary>
        public enum ClientApplicationType
        {
            /// <summary>
            /// Confidential
            /// </summary>
            Confidential,
            /// <summary>
            /// Public
            /// </summary>
            Public
        }
    }
}
