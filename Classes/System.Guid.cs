/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;

namespace Extensions
{
    /// <summary>
    /// Extensions for the Guid class.
    /// </summary>
    [Serializable]
    public static partial class Guid
    {
        /// <summary>
        /// Returns a custom GUID starting with a custom string.
        /// </summary>
        /// <param name="StartWith">A string containing hexadecimal
        /// characters with which the GUID should start.</param>
        /// <returns>A System.Guid that starts with the given characters.</returns>
        public static System.Guid NewCustomGuid(string StartWith = "")
        {
            if (!StartWith.ToLower().ContainsOnly(Constants.HexChars))
            {
                throw new Exception(
                    $"Value [{StartWith}] contains non-hex characters!\n" + 
                    "GUID values can only contain hexadecimal characters.");
            }
            System.Guid result = System.Guid.NewGuid();
            return new System.Guid(StartWith.ToLower() +
                result.ToString().ToLower().Substring(StartWith.Length));
        }
    }
}
