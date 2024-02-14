/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using static Extensions.Core;

namespace Extensions
{
    /// <summary>
    /// Helper methods for console messages.
    /// </summary>
    [Serializable]
    public static partial class ConsoleExtensions
    {
        /// <summary>
        /// Write an array of strings to the console in yellow, resetting
        /// the console foreground color upon completion.
        /// </summary>
        /// <param name="lines">The array of strings to write in the 
        /// message.</param>
        public static void WriteHelp(string[] lines)
        {
            ValidateNoNulls(lines);
            WriteHelp(lines, ConsoleColor.Yellow);
        }

        /// <summary>
        /// Write an array of strings to the console in the given ConsoleColor,
        /// resetting the console foreground color upon completion.
        /// </summary>
        /// <param name="lines">The array of strings to write in the 
        /// message.</param>
        /// <param name="textColor">The ConsoleColor to use for the text
        /// color.</param>
        public static void WriteHelp(string[] lines, 
                                     ConsoleColor textColor)
        {
            ValidateNoNulls(lines, textColor);
            foreach (string str in lines)
            {
                Core.Printf(str, textColor);
            }
        }
    }
}
