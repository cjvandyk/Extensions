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
    public static class Universal
    {
        /// <summary>
        /// The private object used to manage locks on file I/O.
        /// </summary>
        private static object lockManager = new object();

        #region T Load<T>()
        /// <summary>
        /// Universal object method used to serialize ANY object from disk.
        /// </summary>
        /// <typeparam name="T">The type of the target object.</typeparam>
        /// <param name="obj">The triggering object.</param>
        /// <param name="filePath">The path on disk for the save file.</param>
        /// <returns>The object of type T loaded from disk.</returns>
        public static T Load<T>(this T obj,
                                   string filePath = "File.bin")
        {
            try
            {
                lock (lockManager)
                {
                    using (System.IO.Stream stream =
                        System.IO.File.Open(
                            filePath,
                            System.IO.FileMode.Open))
                    {
                        var serializer = new System.Runtime.Serialization.NetDataContractSerializer();
                        obj = (T)serializer.Deserialize(stream);
                        return obj;
                    }
                }
            }
            catch (Exception ex)
            {
                //Dump error.
                ConsoleColor currentColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ForegroundColor = currentColor;
                return default(T);
            }
        }
        #endregion T Load<T>()

        #region T Save<T>()
        /// <summary>
        /// Universal object method used to serialize ANY object to disk.
        /// </summary>
        /// <typeparam name="T">The type of the target object.</typeparam>
        /// <param name="obj">The triggering object.</param>
        /// <param name="filePath">The path on disk for the save file.</param>
        /// <returns>True if save successful, otherwise False.</returns>
        public static bool Save<T>(this T obj,
                                   string filePath = "File.bin")
        {
            try
            {
                lock (lockManager)
                {
                    using (System.IO.Stream stream =
                        System.IO.File.Open(
                            filePath,
                            System.IO.FileMode.Create))
                    {
                        var serializer = new System.Runtime.Serialization.NetDataContractSerializer();
                        serializer.Serialize(stream, obj);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                //Dump error.
                ConsoleColor currentColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ForegroundColor = currentColor;
                return false;
            }
        }
        #endregion T Save<T>()

        #region printf()
        /// <summary>
        /// Simple printf method for console output with color control.  Both
        /// text color and background color is returned to previous state
        /// after the string has been written to console.
        /// </summary>
        /// <param name="msg">String to print to console.</param>
        /// <param name="foreground">Overrideable text color, default to white.</param>
        /// <param name="background">Overrideable background color, default to
        /// black.</param>
        public static void printf(object msg, 
                                  ConsoleColor foreground = ConsoleColor.White, 
                                  ConsoleColor background = ConsoleColor.Black)
        {
            ConsoleColor fore = Console.ForegroundColor;
            ConsoleColor back = Console.BackgroundColor;
            if (foreground != fore)
            {
                Console.ForegroundColor = foreground;
            }
            if (background != back)
            {
                Console.BackgroundColor = background;
            }
            Console.WriteLine(Convert.ToString(msg));
            if (foreground != fore)
            {
                Console.ForegroundColor = fore;
            }
            if (background != back)
            {
                Console.BackgroundColor = back;
            }
        }
        #endregion printf()

        #region ValidateNoNulls()
        /// <summary>
        /// Validate that the given set of parameters doesn't contain any
        /// nulls.  If it does, a ArgumentNullException error is thrown.
        /// </summary>
        /// <param name="parms">The given set of parameters.</param>
        public static void ValidateNoNulls(System.Reflection.ParameterInfo[] parms)
        {
            for (int C = 0; C < parms.Length; C++)
            {
                if (parms[C] == null) throw new ArgumentNullException(parms[C].Name);
            }
        }
        /////////////////////////// Better way above ///////////////////////////
        //public static void ValidateNoNulls(System.Reflection.ParameterInfo[] parms)
        //{
        //    Type t = null;
        //    for (int C = 0; C > parms.Length; C++)
        //    {
        //        t = parms[C].GetType();
        //        if (t == typeof(double?))
        //        {
        //            ((double?)(parms[C].GetType().GetProperties()[0].GetValue(parms[C]))).NoNull();
        //        }
        //    }
        //}
        #endregion ValidateNoNulls()
    }
}
