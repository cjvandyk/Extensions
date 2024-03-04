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
    /// Extensions for the System.Diagnostics.StackTrace class.
    /// </summary>
    [Serializable]
    public static partial class StackTraceExtensions
    {
        #region Parentage
        /// <summary>
        /// A method to return the calling method's name.  Two other methods
        /// exist in the Parentage group i.e. GrandParent() and
        /// GreatGrandParent().  If Generations of callers further back are
        /// needed, this method can be called with the specific number of
        /// generations to traverse up the calling tree.  If the value of the
        /// generations exceed the number of callers in the StackTrace calling
        /// tree, then null will be returned.
        /// </summary>
        /// <param name="stackTrace">This instance of the StackTrace.</param>
        /// <param name="generations">The number of generations to traverse
        /// back up the stack.  Default is 1.</param>
        /// <returns>The name of the calling method x generations ago.</returns>
        public static string Parent(
            this System.Diagnostics.StackTrace stackTrace, 
            int generations = 1)
        {
            ValidateNoNulls(stackTrace, generations);
            try
            {
                return stackTrace.GetFrame(generations).GetMethod().Name;
            }
            catch (Exception ex)
            {
                Err(ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// A method to return the calling method's
        ///                        calling method's name.
        /// </summary>
        /// <returns>The name of the calling method 2 generations ago.</returns>
        public static string GrandParent()
        {
            return Parent(new System.Diagnostics.StackTrace(), 2);
        }

        /// <summary>
        /// A method to return the calling method's
        ///                        calling method's
        ///                        calling method's name.
        /// </summary>
        /// <returns>The name of the calling method 3 generations ago.</returns>
        public static string GreatGrandParent()
        {
            return Parent(new System.Diagnostics.StackTrace(), 3);
        }
        #endregion Parentage
    }
}
