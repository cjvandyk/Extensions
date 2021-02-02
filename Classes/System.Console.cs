#pragma warning disable IDE1006, IDE0017, CS0162, IDE0060, IDE0079 // Naming Styles, Simplify declaration (FQCN used), break after return, Remove unused (string[] args, Remove unnecessary suppression)

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Quix/blob/master/LICENSE
/// </summary>

using System;

namespace Extensions
{
    /// <summary>
    /// Helper methods for console messages.
    /// </summary>
    public static class ConsoleEx
    {
        /// <summary>
        /// Write an array of strings to the console in yellow, resetting
        /// the console foreground color upon completion.
        /// </summary>
        /// <param name="lines">The array of strings to write in the 
        /// message.</param>
        public static void WriteHelp(string[] lines)
        {
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
        public static void WriteHelp(string[] lines, ConsoleColor textColor)
        {
            ConsoleColor color = Console.ForegroundColor;
            Console.ForegroundColor = textColor;
            foreach (string str in lines)
            {
                Console.WriteLine(str);
            }
            Console.ForegroundColor = color;
        }
    }
}
