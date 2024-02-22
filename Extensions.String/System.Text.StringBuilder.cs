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
    /// Extension methods for the System.Text.StringBuilder
    /// classes.
    /// </summary>
    [Serializable]
    public static partial class StringBuilderExtensions
    {
        #region IndexOf()
        /// <summary>
        /// Returns the offset location of the index value.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="index"></param>
        /// <param name="startindex"></param>
        /// <param name="ignorecase"></param>
        /// <returns>The integer offset of the index string in the source
        /// StringBuilder object.</returns>
        public static int IndexOf(this System.Text.StringBuilder str,
                                  string index,
                                  int startindex = 0,
                                  bool ignorecase = true)
        {
            ValidateNoNulls(str, index);
            int counter;
            int targetlength = index.Length;
            if (ignorecase)
            {
                for (int C = startindex; C < ((str.Length - targetlength) + 1); C++)
                {
                    if (Char.ToLower(str[C]) == Char.ToLower(index[0]))
                    {
                        counter = 1;
                        while ((counter < targetlength) && 
                               (Char.ToLower(str[C + counter]) == Char.ToLower(index[counter])))
                        {
                            counter++;
                        }
                        if (counter == targetlength)
                        {
                            return C;
                        }
                    }
                }
                return -1;
            }
            else
            {
                for (int C = startindex; C < ((str.Length - targetlength) + 1); C++)
                {
                    if (str[C] == index[0])
                    {
                        counter = 1;
                        while ((counter < targetlength) && (str[C + counter] == index[counter]))
                        {
                            counter++;
                        }
                        if (counter == targetlength)
                        {
                            return C;
                        }
                    }
                }
            }
            return -1;
        }
        #endregion IndexOf()

        #region Validate()
        /// <summary>
        /// Makes quick work of null validating all parameters you pass to it.
        /// This method takes a variable number of parameters and validates that
        /// all parameters are not null.  If a parameter is found to be null, a
        /// ArgumentNullException is thrown.
        /// For example:
        ///     void MyMethod(string str, double dbl, MyClass cls)
        ///     {
        ///         Universal.ValidateNoNulls(str, dbl, cls);
        ///         ...Your code here...
        ///     }
        /// You do not have to pass all parameters, but can instead do this:
        ///     void MyMethod(string str, double dbl, MyClass cls)
        ///     {
        ///         Universal.ValidateNoNulls(str, cls);
        ///         ...Your code here...
        ///     }
        /// where we chose NOT to validate the double dbl in this case.
        /// </summary>
        /// <param name="parms">The variable set of parameters.</param>
        internal static bool ValidateNoNulls(params object[] parms)
        {
            for (int C = 0; C < parms.Length; C++)
            {
                if (parms[C] == null)
                {
                    throw new ArgumentNullException("Parameter #" + C);
                }
            }
            return true;
        }

        /// <summary>
        /// Makes quick work of validating all parameters you pass to it.
        /// This method takes a variable number of parameters and validates 
        /// all parameters based on ErrorType and object type.  Null validation
        /// is seamless.  If a parameter is found to be null, a
        /// ArgumentNullException is thrown which notes the number of the 
        /// parameter since parmeters are treated as objects and thus the
        /// parameter names are inaccessible.
        /// For example:
        ///     void MyMethod(string str, double dbl, MyClass cls)
        ///     {
        ///         Universal.Validate(ErrorType.Null, str, dbl, cls);
        ///         ...Your code here...
        ///     }
        /// You do not have to pass all parameters, but can instead do this:
        ///     void MyMethod(string str, double dbl, MyClass cls)
        ///     {
        ///         Universal.Validate(ErrorType.Null, str, cls);
        ///         ...Your code here...
        ///     }
        /// where we chose NOT to validate the double dbl in this case.
        /// Alternatively, you can validate that dbl is a non-negative
        /// number by doing this:
        ///     void MyMethod(string str, double dbl, MyClass cls)
        ///     {
        ///         Universal.Validate(
        ///             {ErrorType.Null, ErrorType.NonNegative}, 
        ///             str, cls);
        ///         ...Your code here...
        ///     }
        /// </summary>
        /// <param name="errors">The array of error types to validate.</param>
        /// <param name="parms">The variable set of parameters.</param>
        internal static bool Validate(Constants.ErrorType[] errors,
                                      params object[] parms)
        {
            foreach (Constants.ErrorType error in errors)
            {
                for (int C = 0; C < parms.Length; C++)
                {
                    switch (error)
                    {
                        case Constants.ErrorType.Null:
                            if (parms[C] == null)
                            {
                                throw new ArgumentNullException("Parameter #" + C);
                            }
                            break;
                        case Constants.ErrorType.IntNonNegative:
                            if (parms[C].GetType() == typeof(int))
                            {
                                if ((int)parms[C] < 0)
                                {
                                    throw new ArgumentOutOfRangeException(
                                        "Parameter #" + C,
                                        "Value must be >= 0");
                                }
                            }
                            break;
                    }
                }
            }
            return true;
        }
        #endregion Validate()
    }
}
