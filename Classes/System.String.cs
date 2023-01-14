#pragma warning disable CS1587, CS0162

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Collections.Generic;
using System.Linq;

using static Extensions.Constants;
using static Extensions.Universal;

namespace Extensions
{
    /// <summary>
    /// Extension methods for the System.String and System.Text.StringBuilder
    /// classes.
    /// </summary>
    public static class StringExtensions
    {
        #region BeginsWith()
        /// <summary>
        /// Checks if the current string begins with the given target string.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="target"></param>
        /// <param name="ignorecase"></param>
        /// <returns></returns>
        public static bool BeginsWith(this System.String str,
                                        string target,
                                        bool ignorecase = true)
        {
            ValidateNoNulls(str, target);
            if (target.Length <= str.Length)
            {
                if (ignorecase)
                {
                    if (str.ToLower().Substring(0, target.Length) == target.ToLower())
                    {
                        return true;
                    }
                }
                else
                {
                    if (str.Substring(0, target.Length) == target)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion BeginsWith()

        #region ContainsAny()
        /// <summary>
        /// Checks if the given string contains any of the strings provided in
        /// the IEnumerable.
        /// </summary>
        /// <param name="str">The given string to check.</param>
        /// <param name="chars">The character array to validate against.</param>
        /// <returns>True if the given string contains any characters provided
        /// in the character array, otherwise False.</returns>
        public static bool ContainsAny(this System.String str,
                                       char[] chars)
        {
            ValidateNoNulls(str, chars);
            foreach (char C in chars)
            {
                if (str.Contains(C))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the given string contains any of the strings provided in
        /// the IEnumerable.
        /// </summary>
        /// <param name="str">The given string to check.</param>
        /// <param name="strings">The IEnumerable i.e. List of strings or
        /// string array to validate against.</param>
        /// <returns>True if the given string contains any of the strings
        /// provided in the IEnumerable, otherwise False.</returns>
        public static bool ContainsAny(this System.String str,
                                       IEnumerable<string> strings)
        {
            ValidateNoNulls(str, strings);
            foreach (string C in strings)
            {
                if (str.Contains(C))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the given string contains any of the strings provided in
        /// the IEnumerable.
        /// </summary>
        /// <param name="str">The given string to check.</param>
        /// <param name="chars">The character array to validate against.</param>
        /// <returns>True if the given string contains any characters provided
        /// in the character array, otherwise False.</returns>
        public static bool ContainsAny(this System.Text.StringBuilder str,
                                       char[] chars)
        {
            ValidateNoNulls(str, chars);
            return ContainsAny(str.ToString(), chars);
        }

        /// <summary>
        /// Checks if the given string contains any of the strings provided in
        /// the IEnumerable.
        /// </summary>
        /// <param name="str">The given string to check.</param>
        /// <param name="strings">The IEnumerable i.e. List of strings or
        /// string array to validate against.</param>
        /// <returns>True if the given string contains any of the strings
        /// provided in the IEnumerable, otherwise False.</returns>
        public static bool ContainsAny(this System.Text.StringBuilder str,
                                       IEnumerable<string> strings)
        {
            ValidateNoNulls(str, strings);
            return ContainsAny(str.ToString(), strings);
        }
        #endregion ContainsAny()

        #region ContainsOnly()
        /// <summary>
        /// Checks if the given string contains only the characters provided in
        /// the IEnumerable.
        /// </summary>
        /// <param name="str">The given string to check.</param>
        /// <param name="chars">The character array to validate against.</param>
        /// <returns>True if the given string only contains characters provided
        /// in the IEnumerable, otherwise False.</returns>
        public static bool ContainsOnly(this System.String str,
                                        char[] chars)
        {
            ValidateNoNulls(str, chars);
            string remain = str;
            foreach (char C in chars)
            {
                remain = remain.Replace(C.ToString(), "");
            }
            remain = remain.Trim();
            if (remain == "")
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the given string contains only the strings provided in
        /// the IEnumerable.
        /// </summary>
        /// <param name="str">The given string to check.</param>
        /// <param name="strings">The IEnumerable i.e. List of strings or
        /// string array to validate against.</param>
        /// <returns>True if the given string only contains strings provided
        /// in the IEnumerable, otherwise False.</returns>
        public static bool ContainsOnly(this System.String str,
                                        IEnumerable<string> strings)
        {
            ValidateNoNulls(str, strings);
            string remain = str;
            foreach (string C in strings)
            {
                remain = remain.Replace(C, "");
            }
            remain = remain.Trim();
            if (remain == "")
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the given string contains only the characters provided in
        /// the IEnumerable.
        /// </summary>
        /// <param name="str">The given string to check.</param>
        /// <param name="chars">The character array to validate against.</param>
        /// <returns>True if the given string only contains characters provided
        /// in the IEnumerable, otherwise False.</returns>
        public static bool ContainsOnly(this System.Text.StringBuilder str,
                                        char[] chars)
        {
            ValidateNoNulls(str, chars);
            return ContainsOnly(str.ToString(), chars);
        }

        /// <summary>
        /// Checks if the given string contains only the strings provided in
        /// the IEnumerable.
        /// </summary>
        /// <param name="str">The given string to check.</param>
        /// <param name="strings">The IEnumerable i.e. List of strings or
        /// string array to validate against.</param>
        /// <returns>True if the given string only contains strings provided
        /// in the IEnumerable, otherwise False.</returns>
        public static bool ContainsOnly(this System.Text.StringBuilder str,
                                        IEnumerable<string> strings)
        {
            ValidateNoNulls(str, strings);
            return ContainsOnly(str.ToString(), strings);
        }
        #endregion ContainsOnly()

        #region DoubleQuote()
        /// <summary>
        /// Return the given string encased in double quotes.
        /// </summary>
        /// <param name="str">The given string to be quoted.</param>
        /// <returns>The given string encased in double quotes.</returns>
        public static string DoubleQuote(this System.String str)
        {
            ValidateNoNulls(str);
            return ('"' + str + '"');
        }

        /// <summary>
        /// Return the given string encased in double quotes.
        /// </summary>
        /// <param name="str">The given string to be quoted.</param>
        /// <returns>The given string encased in double quotes.</returns>
        public static string DoubleQuote(this System.Text.StringBuilder str)
        {
            ValidateNoNulls(str);
            return DoubleQuote(str.ToString());
        }
        #endregion DoubleQuote()

        #region Encrypt

        //public static string Encrypt(this System.String str, 
        //                             string password, 
        //                             Constants.EncryptionProvider provider = 
        //                                Constants.EncryptionProvider.SHA512)
        //{
        //    System.Security.Cryptography.SHA512CryptoServiceProvider cryptoServiceProvider =
        //        new System.Security.Cryptography.SHA512CryptoServiceProvider();
        //    System.Text.Encoding encoding = System.Text.Encoding.UTF8;
        //    System.Security.Cryptography.Rfc2898DeriveBytes key =
        //        new System.Security.Cryptography.Rfc2898DeriveBytes(password, salt);
        //    byte[] raw = encoding.GetBytes(password);
        //    byte[] hashKey = cryptoServiceProvider.ComputeHash(raw);
        //    byte[] hashIV = hashKey.CopyTo(16);
        //    byte[] encrypted;
        //    using (System.Security.Cryptography.RijndaelManaged managed =
        //        new System.Security.Cryptography.RijndaelManaged())
        //    {
        //        managed.Key = hashKey;
        //        managed.IV = hashIV;
        //        System.Security.Cryptography.ICryptoTransform encryptor =
        //            managed.CreateEncryptor(managed.Key, managed.IV);
        //        using (System.IO.MemoryStream memoryStream = 
        //            new System.IO.MemoryStream())
        //        {
        //            using (System.Security.Cryptography.CryptoStream cryptoStream =
        //                new System.Security.Cryptography.CryptoStream(memoryStream, 
        //                                                              encryptor, 
        //                                                              System.Security.Cryptography.CryptoStreamMode.Write))
        //            {
        //                using (System.IO.StreamWriter streamWriter =
        //                    new System.IO.StreamWriter(cryptoStream))
        //                {
        //                    streamWriter.Write(str);
        //                }
        //                encrypted = memoryStream.ToArray();
        //            }
        //        }
        //    }
        //    return Convert.ToBase64String(encrypted);
        //}

        //public static string Decrypt(this System.String str,
        //                     string password,
        //                     Constants.EncryptionProvider provider =
        //                        Constants.EncryptionProvider.SHA512)
        //{
        //    System.Security.Cryptography.SHA512CryptoServiceProvider cryptoServiceProvider =
        //        new System.Security.Cryptography.SHA512CryptoServiceProvider();
        //    System.Text.Encoding encoding = System.Text.Encoding.UTF8;
        //    byte[] raw = encoding.GetBytes(password);
        //    byte[] hashKey = cryptoServiceProvider.ComputeHash(raw);
        //    byte[] hashIV = hashKey.CopyTo(16);
        //    byte[] encrypted = Convert.FromBase64String(str);
        //    string decrypted = null;
        //    using (System.Security.Cryptography.RijndaelManaged managed =
        //        new System.Security.Cryptography.RijndaelManaged())
        //    {
        //        managed.Key = hashKey;
        //        managed.IV = hashIV;
        //        System.Security.Cryptography.ICryptoTransform decryptor =
        //            managed.CreateDecryptor(managed.Key, managed.IV);
        //        using (System.IO.MemoryStream memoryStream =
        //            new System.IO.MemoryStream(encrypted))
        //        {
        //            using (System.Security.Cryptography.CryptoStream cryptoStream =
        //                new System.Security.Cryptography.CryptoStream(memoryStream,
        //                                                              decryptor,
        //                                                              System.Security.Cryptography.CryptoStreamMode.Read))
        //            {
        //                using (System.IO.StreamReader streamReader =
        //                    new System.IO.StreamReader(cryptoStream))
        //                {
        //                    decrypted = streamReader.ReadToEnd();
        //                }
        //            }
        //        }
        //    }
        //    return decrypted;
        //}

        #endregion Encrypt

        #region ExceedsLength()
        /// <summary>
        /// Checks if a referenced offset exceeds the length of the string.
        /// </summary>
        /// <param name="str">The current string to check against.</param>
        /// <param name="offset">The referenced offset value.</param>
        /// <param name="increment">Should the offset be incremented before
        /// the length comparison takes place.</param>
        /// <returns></returns>
        public static bool ExceedsLength(this System.String str, 
                                        ref int offset, 
                                        bool increment = true)
        {
            if (increment)
            {
                offset++;
            }
            if (offset >= str.Length)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if a referenced offset exceeds the length of the string.
        /// </summary>
        /// <param name="str">The current string to check against.</param>
        /// <param name="offset">The referenced offset value.</param>
        /// <param name="increment">Should the offset be incremented before
        /// the length comparison takes place.</param>
        /// <returns></returns>
        public static bool ExceedsLength(this System.Text.StringBuilder str, 
                                        ref int offset, 
                                        bool increment = true)
        {
            return ExceedsLength(str.ToString(), ref offset, increment);
        }
        #endregion ExceedsLength()

        #region GetSiteUrl()
        /// <summary>
        /// Get the tenant root for the given string object containing a URL.
        /// For example:
        ///   "https://blog.cjvandyk.com/sites/Approval".GetTenantUrl()
        ///   will return "https://blog.cjvandyk.com".
        /// </summary>
        /// <param name="url">The System.String object containing the URL
        /// from which the tenant root is to be extracted.</param>
        /// <returns>The tenant root of the URL given the URL string.</returns>
        public static string GetSiteUrl(this System.String url)
        {
            return url.Substring(-1, "/", SubstringType.LeftOfIndex, 5);
        }
        #endregion GetSiteUrl()

        #region GetTenantUrl()
        /// <summary>
        /// Get the tenant root for the given string object containing a URL.
        /// For example:
        ///   "https://blog.cjvandyk.com/sites/Approval".GetTenantUrl()
        ///   will return "https://blog.cjvandyk.com".
        /// </summary>
        /// <param name="url">The System.String object containing the URL
        /// from which the tenant root is to be extracted.</param>
        /// <returns>The tenant root of the URL given the URL string.</returns>
        public static string GetTenantUrl(this System.String url)
        {
            return GetUrlRoot(url);
        }
        #endregion GetTenantUrl()

        #region GetUrlRoot()
        /// <summary>
        /// Get the URL root for the given string object containing a URL.
        /// For example:
        ///   "https://blog.cjvandyk.com".GetUrlRoot() 
        ///   will return "https://blog.cjvandyk.com" whereas
        ///   "https://blog.cjvandyk.com/sites/Approval".GetUrlRoot()
        ///   will also return "https://blog.cjvandyk.com".
        /// </summary>
        /// <param name="url">The System.String object containing the URL
        /// from which the root is to be extracted.</param>
        /// <returns>The root of the URL given the URL string.</returns>
        public static string GetUrlRoot(this System.String url)
        {
            ValidateNoNulls(url);
            string root = url.ToLower().Replace("https://", "");
            return ("https://" + root.Substring(0, root.IndexOf('/')));
        }

        /// <summary>
        /// Get the URL root for the given string builder object containing a
        /// URL.  For example:
        ///   "https://blog.cjvandyk.com".GetUrlRoot() 
        ///   will return "https://blog.cjvandyk.com" whereas
        ///   "https://blog.cjvandyk.com/sites/Approval".GetUrlRoot()
        ///   will also return "https://blog.cjvandyk.com".
        /// </summary>
        /// <param name="url">The System.Text.StringBuilder object containing
        /// the URL from which the root is to be extracted.</param>
        /// <returns>The root of the URL given the URL string.</returns>
        public static string GetUrlRoot(this System.Text.StringBuilder url)
        {
            ValidateNoNulls(url);
            return GetUrlRoot(url.ToString());
        }
        #endregion GetUrlRoot()

        #region HasLower()
        /// <summary>
        /// Check if given System.String object contains lower case.
        /// </summary>
        /// <param name="str">The string object to check.</param>
        /// <param name="ignoreSpaces">Remove spaces before checking?</param>
        /// <returns>True if the object contains any lower case, else False.</returns>
        public static bool HasLower(this System.String str,
                                   bool ignoreSpaces = true)
        {
            ValidateNoNulls(str, ignoreSpaces);
            return (ignoreSpaces ? str.Replace(" ", "") : str)
                    .ToCharArray()
                    .Select(C => (int)C)
                    .Any(C => C >= 97 && C <= 122);
        }

        /// <summary>
        /// Check if given System.Text.StringBuilder object contains lower case.
        /// </summary>
        /// <param name="str">The string object to check.</param>
        /// <returns>True if the string contains any lower case, else False.</returns>
        public static bool HasLower(this System.Text.StringBuilder str)
        {
            ValidateNoNulls(str);
            return HasLower(str.ToString());
        }
        #endregion HasLower()

        #region HasNumeric()
        /// <summary>
        /// Checks if the given string contains any numeric characters.
        /// </summary>
        /// <param name="str">The given string object to check.</param>
        /// <param name="ignoreSpaces">Remove spaces before compare?</param>
        /// <returns>True if any characters in the given string are numeric,
        /// else False.</returns>
        public static bool HasNumeric(this System.String str,
                                      bool ignoreSpaces = true)
        {
            ValidateNoNulls(str, ignoreSpaces);
            return (ignoreSpaces ? str.Replace(" ", "") : str)
                    .ToCharArray()
                    .Any(Char.IsDigit);
        }

        /// <summary>
        /// Checks if the given string contains any numeric characters.
        /// </summary>
        /// <param name="str">The given string builder object to check.</param>
        /// <param name="ignoreSpaces">Remove spaces before compare?</param>
        /// <returns>True if any characters in the given string are numeric,
        /// else False.</returns>
        public static bool HasNumeric(this System.Text.StringBuilder str,
                                     bool ignoreSpaces = true)
        {
            ValidateNoNulls(str, ignoreSpaces);
            return HasNumeric(str.ToString(),
                             ignoreSpaces);
        }
        #endregion HasNumeric()

        #region HasSymbol()
        /// <summary>
        /// Checks if the given System.String object contains symbols or 
        /// special characters.
        /// </summary>
        /// <param name="str">The given string object to check.</param>
        /// <param name="ignoreSpaces">Remove spaces before compare?</param>
        /// <returns>True if any characters in the given string are 
        /// symbols or special characters, else False.</returns>
        public static bool HasSymbol(this System.String str,
                                          bool ignoreSpaces = true)
        {
            ValidateNoNulls(str, ignoreSpaces);
            return (!(ignoreSpaces ? str.Replace(" ", "") : str)
                    .ToCharArray()
                    .All(Char.IsLetterOrDigit));
        }

        /// <summary>
        /// Checks if the given System.Text.StringBuilder object contains
        /// symbols or special characters.
        /// </summary>
        /// <param name="str">The given string object to check.</param>
        /// <param name="Classic">Use non-RegEx method.</param>
        /// <param name="ignoreSpaces">Remove spaces before compare?</param>
        /// <returns>True if all characters in the given string contains
        /// any symbols or special characters, else False.</returns>
        public static bool HasSymbol(this System.Text.StringBuilder str,
                                          bool Classic = false,
                                          bool ignoreSpaces = true)
        {
            ValidateNoNulls(str, Classic, ignoreSpaces);
            return HasSymbol(str.ToString(),
                                  ignoreSpaces);
        }
        #endregion HasSymbol()

        #region HasUpper()
        /// <summary>
        /// Check if given System.String object contains upper case.
        /// </summary>
        /// <param name="str">The string object to check.</param>
        /// <param name="ignoreSpaces">Remove spaces before checking?</param>
        /// <returns>True if the string contains any upper case, else False.</returns>
        public static bool HasUpper(this System.String str,
                                    bool ignoreSpaces = true)
        {
            ValidateNoNulls(str, ignoreSpaces);
            return (ignoreSpaces ? str.Replace(" ", "") : str)
                    .ToCharArray()
                    .Select(C => (int)C)
                    .Any(C => C >= 65 && C <= 90);
        }

        /// <summary>
        /// Check if given System.Text.StringBuilder object contains upper case.
        /// </summary>
        /// <param name="str">The string object to check.</param>
        /// <returns>True if the string contains any upper case, else False.</returns>
        public static bool HasUpper(this System.Text.StringBuilder str)
        {
            ValidateNoNulls(str);
            return HasUpper(str.ToString());
        }
        #endregion HasUpper()

        #region HtmlDecode()
        /// <summary>
        /// Decode the HTML escaped components in a given string.
        /// </summary>
        /// <param name="str">The given source string to decode.</param>
        /// <returns>The given source string without HTML escaped components.</returns>
        public static string HtmlDecode(this System.String str)
        {
            ValidateNoNulls(str);
#if NET5_0_OR_GREATER
            return System.Web.HttpUtility.HtmlDecode(str);
#else
            return System.Net.WebUtility.HtmlDecode(str);
#endif
        }

        /// <summary>
        /// Decode the HTML escaped components in a given string.
        /// </summary>
        /// <param name="str">The given source string to decode.</param>
        /// <returns>The given source string without HTML escaped components.</returns>
        public static string HtmlDecode(this System.Text.StringBuilder str)
        {
            ValidateNoNulls(str);
            return HtmlDecode(str.ToString());
        }
        #endregion HtmlDecode()

        #region HtmlEncode()
        /// <summary>
        /// Encode the given string to be HTML safe.
        /// </summary>
        /// <param name="str">The given source string to encode.</param>
        /// <returns>The given source string in HTML safe format.</returns>
        public static string HtmlEncode(this System.String str)
        {
            ValidateNoNulls(str);
#if NET5_0_OR_GREATER
            return System.Web.HttpUtility.HtmlEncode(str);
#else
            return System.Net.WebUtility.HtmlEncode(str);
#endif
        }

        /// <summary>
        /// Encode the given string to be HTML safe.
        /// </summary>
        /// <param name="str">The given source string to encode.</param>
        /// <returns>The given source string in HTML safe format.</returns>
        public static string HtmlEncode(this System.Text.StringBuilder str)
        {
            ValidateNoNulls(str);
            return HtmlEncode(str.ToString());
        }
        #endregion HtmlEncode()

        #region IsAlphabetic()
        /// <summary>
        /// Checks if the given string contains all alphabetic characters.
        /// </summary>
        /// <param name="str">The given string object to check.</param>
        /// <param name="Classic">Switch to force RegEx comparison instead of
        /// Linq.</param>
        /// <param name="ignoreSpaces">Remove spaces before compare?</param>
        /// <returns>True if all characters in the given string are alphabetic,
        /// else False.</returns>
        public static bool IsAlphabetic(this System.String str, 
                                        bool Classic = false,
                                        bool ignoreSpaces = true)
        {
            ValidateNoNulls(str, Classic, ignoreSpaces);
            if (Classic)  //No LINQ available e.g. .NET 2.0
            {
                return System.Text.RegularExpressions.Regex.IsMatch(
                    (ignoreSpaces ? str.Replace(" ", "") : str), 
                    @"^[a-zA-Z]+$");
            }
            else  //This method is on average 670% faster than RegEx method.
            {
                return (ignoreSpaces ? str.Replace(" ", "") : str)
                        .ToCharArray()
                        .All(Char.IsLetter);
            }
        }

        /// <summary>
        /// Checks if the given string contains all alphabetic characters.
        /// </summary>
        /// <param name="str">The given string builder object to check.</param>
        /// <param name="Classic">Switch to force RegEx comparison instead
        /// of Linq.</param>
        /// <param name="ignoreSpaces">Remove spaces before compare?</param>
        /// <returns>True if all characters in the given string are alphabetic,
        /// else False.</returns>
        public static bool IsAlphabetic(this System.Text.StringBuilder str, 
                                        bool Classic = false,
                                        bool ignoreSpaces = true)
        {
            ValidateNoNulls(str, Classic, ignoreSpaces);
            return IsAlphabetic(str.ToString(), 
                                Classic, 
                                ignoreSpaces);
        }
        #endregion IsAlphabetic()

        #region IsAlphaNumeric()
        /// <summary>
        /// Checks if the given string contains only alphabetic and numeric 
        /// characters.
        /// </summary>
        /// <param name="str">The given string object to check.</param>
        /// <param name="Classic">Switch to force RegEx comparison instead of 
        /// Linq.</param>
        /// <param name="ignoreSpaces">Remove spaces before compare?</param>
        /// <returns>True if all characters in the given string are either 
        /// alphabetic or numeric, else False.</returns>
        public static bool IsAlphaNumeric(this System.String str,
                                          bool Classic = false,
                                          bool ignoreSpaces = true)
        {
            ValidateNoNulls(str, Classic, ignoreSpaces);
            if (Classic)  //No LINQ available e.g. .NET 2.0
            {
                return System.Text.RegularExpressions.Regex.IsMatch(
                    (ignoreSpaces ? str.Replace(" ", "") : str),
                    @"^[a-zA-Z0-9]+$");
            }
            else  //This method is on average 670% faster than RegEx method.
            {
                return (ignoreSpaces ? str.Replace(" ", "") : str)
                        .ToCharArray()
                        .All(Char.IsLetterOrDigit);
            }
        }

        /// <summary>
        /// Checks if the given string contains only alphabetic and numeric 
        /// characters.
        /// </summary>
        /// <param name="str">The given string builder object to check.</param>
        /// <param name="Classic">Switch to force RegEx comparison instead of 
        /// Linq.</param>
        /// <param name="ignoreSpaces">Remove spaces before compare?</param>
        /// <returns>True if all characters in the given string are either 
        /// alphabetic or numeric, else False.</returns>
        public static bool IsAlphaNumeric(this System.Text.StringBuilder str,
                                          bool Classic = false,
                                          bool ignoreSpaces = true)
        {
            ValidateNoNulls(str, Classic, ignoreSpaces);
            return IsAlphaNumeric(str.ToString(), 
                                  Classic, 
                                  ignoreSpaces);
        }
        #endregion IsAlphaNumeric()

        #region IsChar()
        /// <summary>
        /// Check if the given string contains only the characters in the 
        /// Chars array being passed.
        /// </summary>
        /// <param name="str">The given string object to check.</param>
        /// <param name="Chars">The array of valid characters that are checked
        /// in the string.</param>
        /// <param name="Classic">Switch to force RegEx comparison instead of
        /// Linq.</param>
        /// <returns>True if the given string contains only characters in the 
        /// Chars array, else False.</returns>
        public static bool IsChar(this System.String str,
                                  char[] Chars,
                                  bool Classic = false)
        {
            ValidateNoNulls(str, Chars, Classic);
            if (Classic)  //No LINQ available e.g. .NET 2.0
            {
                string comparor = @"^[";
                foreach (char c in Chars)
                {
                    comparor += c;
                }
                comparor += "]+$";
                return System.Text.RegularExpressions.Regex.IsMatch(str,
                                                                    comparor);
            }
            else
            {
                foreach (char c in Chars)
                {
                    if (!str.Contains(c))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Check if the given string contains only the characters in the
        /// Chars array being passed.
        /// </summary>
        /// <param name="str">The given string builder object to check.</param>
        /// <param name="Chars">The array of valid characters that are checked
        /// in the string.</param>
        /// <param name="Classic">Switch to force RegEx comparison instead of
        /// Linq.</param>
        /// <returns>True if the given string contains only characters in the
        /// Chars array, else False.</returns>
        public static bool IsChar(this System.Text.StringBuilder str,
                                  char[] Chars,
                                  bool Classic = false)
        {
            ValidateNoNulls(str, Chars, Classic);
            return IsChar(str.ToString(), Chars, Classic);
        }
        #endregion IsChar()

        #region IsEmail()
        /// <summary>
        /// Checks if a given System.String object is an email address.
        /// </summary>
        /// <param name="str">The System.String to validate as email.</param>
        /// <returns>True if email, false if not.</returns>
        public static bool IsEmail(this System.String str)
        {
            ValidateNoNulls(str);
            try
            {
                var email = new System.Net.Mail.MailAddress(str);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a given System.Text.StringBuilder object is an email
        /// address.
        /// </summary>
        /// <param name="str">The System.Text.StringBuilder to validate as
        /// email.</param>
        /// <returns>True if email, false if not.</returns>
        public static bool IsEmail(this System.Text.StringBuilder str)
        {
            ValidateNoNulls(str);
            return IsEmail(str.ToString());
        }
        #endregion IsEmail()

        #region IsLower()
        /// <summary>
        /// Check if given System.String object is all lower case.
        /// </summary>
        /// <param name="str">The string object to check.</param>
        /// <param name="ignoreSpaces">Remove spaces before checking?</param>
        /// <returns>True if the object is all lower case, else False.</returns>
        public static bool IsLower(this System.String str, 
                                   bool ignoreSpaces = true)
        {
            ValidateNoNulls(str, ignoreSpaces);
            return (ignoreSpaces ? str.Replace(" ", "") : str)
                    .ToCharArray()
                    .Select(C => (int)C)
                    .All(C => C >= 97 && C <= 122);
        }

        /// <summary>
        /// Check if given System.Text.StringBuilder object is all lower case.
        /// </summary>
        /// <param name="str">The string object to check.</param>
        /// <returns>True if the entire string is lower case, else False.</returns>
        public static bool IsLower(this System.Text.StringBuilder str)
        {
            ValidateNoNulls(str);
            return IsLower(str.ToString());
        }
        #endregion IsLower()

        #region IsNumeric()
        /// <summary>
        /// Checks if the given string contains all numeric characters.
        /// </summary>
        /// <param name="str">The given string object to check.</param>
        /// <param name="Classic">Switch to force RegEx comparison instead
        /// of Linq.</param>
        /// <param name="ignoreSpaces">Remove spaces before compare?</param>
        /// <returns>True if all characters in the given string are numeric,
        /// else False.</returns>
        public static bool IsNumeric(this System.String str, 
                                     bool Classic = false,
                                     bool ignoreSpaces = true)
        {
            ValidateNoNulls(str, Classic, ignoreSpaces);
            if (Classic)  //No LINQ available e.g. .NET 2.0
            {
                return System.Text.RegularExpressions.Regex.IsMatch(
                    (ignoreSpaces ? str.Replace(" ", "") : str),
                    @"^[0-9]+$");
            }
            else  //This method is on average 670% faster than RegEx method.
            {
                return (ignoreSpaces ? str.Replace(" ", "") : str)
                        .ToCharArray()
                        .All(Char.IsDigit);
            }
        }

        /// <summary>
        /// Checks if the given string contains all numeric characters.
        /// </summary>
        /// <param name="str">The given string builder object to check.</param>
        /// <param name="Classic">Switch to force RegEx comparison instead of 
        /// Linq.</param>
        /// <param name="ignoreSpaces">Remove spaces before compare?</param>
        /// <returns>True if all characters in the given string are numeric,
        /// else False.</returns>
        public static bool IsNumeric(this System.Text.StringBuilder str, 
                                     bool Classic = false,
                                     bool ignoreSpaces = true)
        {
            ValidateNoNulls(str, Classic, ignoreSpaces);
            return IsNumeric(str.ToString(), 
                             Classic, 
                             ignoreSpaces);
        }
        #endregion IsNumeric()

        #region IsStrong()
        /// <summary>
        /// Checks if the given System.String object is a strong password
        /// string.
        /// </summary>
        /// <param name="str">The given string to check.</param>
        /// <param name="numberCriteriaRequired">The number of strong criteria
        /// that are required to be checked for.</param>
        /// <param name="requireUpper">Are upper case characters required?
        /// Default is Yes.</param>
        /// <param name="requireLower">Are lower case characters required?
        /// Default is Yes.</param>
        /// <param name="requireNumeric">Are numbers required?
        /// Default is Yes.</param>
        /// <param name="requireSymbol">Are symbols or special characters 
        /// required?  Default is Yes.</param>
        /// <returns>True if string matches all require... criteria, 
        /// else False.</returns>
        public static bool IsStrong(this System.String str,
                                    int numberCriteriaRequired = 4,
                                    bool requireUpper = true,
                                    bool requireLower = true,
                                    bool requireNumeric = true,
                                    bool requireSymbol = true)
        {
            ValidateNoNulls(str, numberCriteriaRequired, 
                                      requireUpper, requireLower, 
                                      requireNumeric, requireSymbol);
            if ((numberCriteriaRequired < 1) ||
                (numberCriteriaRequired > 4))
            {
                return false;
            }
            int criteria = numberCriteriaRequired;
            int required = 0;
            if (requireUpper) required++;
            if (requireLower) required++;
            if (requireNumeric) required++;
            if (requireSymbol) required++;

            if (str.HasUpper())
            {
                criteria -= 1;
                if (criteria == 0)
                {
                    if (requireUpper) required--;
                    if (required == 0)
                    {
                        return true;
                    }
                }
                else
                {
                    if (requireUpper) required--;
                }
            }
            else
            {
                if (requireUpper)
                {
                    return false;
                }
            }

            if (str.HasLower())
            {
                criteria -= 1;
                if (criteria == 0)
                {
                    if (requireLower) required--;
                    if (required == 0)
                    {
                        return true;
                    }
                }
                else
                {
                    if (requireLower) required--;
                }
            }
            else
            {
                if (requireLower)
                {
                    return false;
                }
            }

            if (str.HasNumeric())
            {
                criteria -= 1;
                if (criteria == 0)
                {
                    if (requireNumeric) required--;
                    if (required == 0)
                    {
                        return true;
                    }
                }
                else
                {
                    if (requireNumeric) required--;
                }
            }
            else
            {
                if (requireNumeric)
                {
                    return false;
                }
            }

            if (str.HasSymbol())
            {
                criteria -= 1;
                if (criteria == 0)
                {
                    if (requireSymbol) required--;
                    if (required == 0)
                    {
                        return true;
                    }
                }
                else
                {
                    if (requireSymbol) required--;
                }
            }
            else
            {
                if (requireSymbol)
                {
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the given System.Test.StringBuilder object is a strong 
        /// password string.
        /// </summary>
        /// <param name="str">The given string to check.</param>
        /// <param name="numberCriteriaRequired">The number of strong criteria
        /// that are required to be checked for.</param>
        /// <param name="requireUpper">Are upper case characters required?
        /// Default is Yes.</param>
        /// <param name="requireLower">Are lower case characters required?
        /// Default is Yes.</param>
        /// <param name="requireNumeric">Are numbers required?
        /// Default is Yes.</param>
        /// <param name="requireSymbol">Are symbols or special characters 
        /// required?  Default is Yes.</param>
        /// <returns>True if string matches all require... criteria, 
        /// else False.</returns>
        public static bool IsStrong(this System.Text.StringBuilder str,
                                    int numberCriteriaRequired = 4,
                                    bool requireUpper = true,
                                    bool requireLower = true,
                                    bool requireNumeric = true,
                                    bool requireSymbol = true)
        {
            ValidateNoNulls(str, numberCriteriaRequired, 
                                      requireUpper, requireLower, 
                                      requireNumeric, requireSymbol);
            return IsStrong(str,
                            numberCriteriaRequired,
                            requireUpper,
                            requireLower,
                            requireNumeric,
                            requireSymbol);
        }
        #endregion IsStrong()

        #region IsUpper()
        /// <summary>
        /// Check if given System.String object is all upper case.
        /// </summary>
        /// <param name="str">The string object to check.</param>
        /// <param name="ignoreSpaces">Remove spaces before checking?</param>
        /// <returns>True if the entire string is upper case, else False.</returns>
        public static bool IsUpper(this System.String str, 
                                   bool ignoreSpaces = true)
        {
            ValidateNoNulls(str, ignoreSpaces);
            return (ignoreSpaces ? str.Replace(" ", "") : str)
                    .ToCharArray()
                    .Select(C => (int)C)
                    .All(C => C >= 65 && C <= 90);
        }

        /// <summary>
        /// Check if given System.Text.StringBuilder object is all upper case.
        /// </summary>
        /// <param name="str">The string object to check.</param>
        /// <returns>True if the entire string is upper case, else False.</returns>
        public static bool IsUpper(this System.Text.StringBuilder str)
        {
            ValidateNoNulls(str);
            return IsUpper(str.ToString());
        }
        #endregion IsUpper()

        #region IsUrlRoot()
        /// <summary>
        /// Check if the given string object containing a URL, is that of the
        /// URL root only.  Returns True if so, False if not.  For example:
        ///   "https://blog.cjvandyk.com".IsUrlRootOnly() 
        ///   will return True whereas
        ///   "https://blog.cjvandyk.com/sites/Approval".IsUrlRootOnly()
        ///   will return False.
        /// </summary>
        /// <param name="url">The System.String object containing the URL to 
        /// be checked.</param>
        /// <returns>True if the URL is a root, False if not.</returns>
        public static bool IsUrlRoot(this System.String url)
        {
            ValidateNoNulls(url);
            if (url.ToLower()
                   .TrimEnd('/')
                   .Replace("https://", "")
                   .Replace("http://", "")
                   .IndexOf('/') == -1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if the given string builder object containing a URL, is that
        ///  of the URL root only.  Returns True if so, False if not.  
        ///  For example:
        ///   "https://blog.cjvandyk.com".IsUrlRootOnly() 
        ///   will return True whereas
        ///   "https://blog.cjvandyk.com/sites/Approval".IsUrlRootOnly()
        ///   will return False.
        /// </summary>
        /// <param name="url">The System.Text.StringBuilder object containing 
        /// the URL to be checked.</param>
        /// <returns>True if the URL is a root, False if not.</returns>
        public static bool IsUrlRoot(this System.Text.StringBuilder url)
        {
            ValidateNoNulls(url);
            return IsUrlRoot(url.ToString());
        }
        #endregion IsUrlRoot()

        #region IsVowel()
        /// <summary>
        /// Checks if the given System.Char is an English vowel.
        /// </summary>
        /// <param name="C">The char to check.</param>
        /// <returns>True if it's a vowel, else False.</returns>
        public static bool IsVowel(this System.Char C)
        {
            ValidateNoNulls(C);
            switch (C)
            {
                case 'a':
                case 'e':
                case 'i':
                case 'o':
                case 'u':
                case 'y':
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if the given System.String is an English vowel.
        /// This allows the developer the ability to check a string without
        /// having to first convert to a char e.g. as a substring return.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <returns>True if it's a vowel, else False.</returns>
        public static bool IsVowel(this System.String str)
        {
            ValidateNoNulls(str);
            return IsVowel(Convert.ToChar(str));
        }

        /// <summary>
        /// Checks if the given System.Test.StringBuilder is an English vowel.
        /// This allows the developer the ability to check a string without
        /// having to first convert to a char e.g. as a substring return.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <returns>True if it's a vowel, else False.</returns>
        public static bool IsVowel(this System.Text.StringBuilder str)
        {
            ValidateNoNulls(str);
            return IsVowel(Convert.ToChar(str.ToString()));
        }
        #endregion IsVowel()

        #region IsZipCode()
        /// <summary>
        /// Checks if the given System.String object is in the valid format
        /// of a United States zip code i.e. nnnnn-nnnn or just nnnnn.
        /// </summary>
        /// <param name="str">The given string object to check.</param>
        /// <returns>True if in valid format, else False.</returns>
        public static bool IsZipCode(this System.String str)
        {
            ValidateNoNulls(str);
            string[] parts = str.Split('-');
            if (parts.Length == 1)
            {
                return IsZipCode5Digits(str);
            }
            else
            {
                if (parts.Length == 2)
                {
                    return (str.Length == 10 && 
                            IsZipCode5Digits(parts[0]) && 
                            parts[1].Length == 4 && 
                            int.TryParse(parts[1], out _));
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the given System.Text.StringBuilder object is in the
        /// valid format of a United States zip code i.e. nnnnn-nnnn or
        /// just nnnnn.
        /// </summary>
        /// <param name="str">The given string object to check.</param>
        /// <returns>True if in valid format, else False.</returns>
        public static bool IsZipCode(this System.Text.StringBuilder str)
        {
            ValidateNoNulls(str);
            return IsZipCode(str.ToString());
        }

        /// <summary>
        /// Checks if the given System.String is 5 digits long and numeric.
        /// </summary>
        /// <param name="str">The given string object to check.</param>
        /// <returns>True if a 5 digit numeric, else False.</returns>
        private static bool IsZipCode5Digits(this System.String str)
        {
            ValidateNoNulls(str);
            return (str.Length == 5 && 
                    int.TryParse(str, out _));
        }
        #endregion IsZipCode()

        #region Left()
        /// <summary>
        /// Returns text to the left of the index string.  Use negative values
        /// for occurrence if the occurrence count should start from the end
        /// instead of its default from the beginning of the string.
        /// </summary>
        /// <param name="str">A System.String object.</param>
        /// <returns>Returns text to the left of the index string.  Use negative values
        /// for occurrence if the occurrence count should start from the end
        /// instead of its default from the beginning of the string.</returns>
        public static string Left(this System.String str, 
                                  string index, 
                                  int occurrence = 1)
        {
            ValidateNoNulls(str, index, occurrence);
            if (str.IndexOf(index) > 0)
            {
                if (occurrence == 1)
                {
                    return str.Substring(0, str.IndexOf(index));
                }
                if (occurrence == -1)
                {
                    return str.Substring(0, str.LastIndexOf(index));
                }
                if (occurrence > 1)
                {
                    string remainder = str.Substring(str.IndexOf(index));
                    for (int C = 1; C <= occurrence; C++)
                    {
                        remainder = remainder.Substring(remainder.IndexOf(index) + 1);
                    }
                    return str.Replace(index + remainder, "");
                }
                if (occurrence < -1)
                {
                    string remainder = str.Substring(0, str.LastIndexOf(index));
                    for (int C = -1; C > occurrence; C--)
                    {
                        remainder = remainder.Substring(0, remainder.LastIndexOf(index));
                        if (remainder.IndexOf(index) == -1)
                        {
                            return remainder;
                        }
                    }
                    return remainder;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns text to the left of the index string.
        /// </summary>
        /// <param name="str">A System.String object.</param>
        /// <returns>Returns text to the left of the index string.</returns>
        public static string Left(this System.Text.StringBuilder str,
                                  string index,
                                  int occurrence)
        {
            ValidateNoNulls(str, index, occurrence);
            if (occurrence == 1)
            {
                return str.Substring(0, str.IndexOf(index));
            }
            else
            {
                if (occurrence == -1)
                {
                    //return str.Substring(0, str.LastIndexOf(index));
                }
            }
            if (occurrence > 1)
            {
                string remainder = str.Substring(str.IndexOf(index));
                for (int C = 1; C <= occurrence; C++)
                {
                    remainder = remainder.Substring(remainder.IndexOf(index));
                }
                return remainder;
            }
            return null;
        }
        #endregion Left()

        #region Lines()
        /// <summary>
        /// Returns the number of sentences in the given string object.
        /// </summary>
        /// <param name="str">A System.String object.</param>
        /// <returns>The number of sentences in the given object.</returns>
        public static int Lines(this System.String str)
        {
            ValidateNoNulls(str);
            return str.Split(new char[] { '\n' },
                             StringSplitOptions.RemoveEmptyEntries)
                      .Length;
        }

        /// <summary>
        /// Returns the number of sentences in the given string builder object.
        /// </summary>
        /// <param name="str">A System.Text.StringBuilder object.</param>
        /// <returns>The number of sentences in the given object.</returns>
        public static int Lines(this System.Text.StringBuilder str)
        {
            ValidateNoNulls(str);
            return Lines(str.ToString());
        }
        #endregion Lines()

        #region LoremIpsum()
        /// <summary>
        /// Returns a string containing 1 - 10 paragraphs of dummy text
        /// in lorem ipsum style.
        /// </summary>
        /// <param name="str">The System.String object to be populated with
        /// the dummy text.</param>
        /// <param name="Paragraphs">An integer with the number of paragraphs
        /// to be returned.  Presently supports 1-10</param>
        /// <returns>The string containing the generated dummy text.</returns>
        public static string LoremIpsum(this System.String str, 
                                        int Paragraphs)
        {
            Validate(ErrorTypeAll, str, Paragraphs);
            str = null;
            for (int C = 0; C < (Paragraphs > 10 ? 10 : Paragraphs); C++)
            {
                str += Extensions.Constants.LoremIpsum[C] + '\n' + '\n';
            }
            return str;
        }

        /// <summary>
        /// Returns a string containing 1 - 10 paragraphs of dummy text
        /// in lorem ipsum style.
        /// </summary>
        /// <param name="str">The System.String object to be populated with
        /// the dummy text.</param>
        /// <param name="Paragraphs">An integer with the number of paragraphs
        /// to be returned.  Presently supports 1-10</param>
        /// <returns>The string containing the generated dummy text.</returns>
        public static string LoremIpsum(this System.Text.StringBuilder str,
                                        int Paragraphs)
        {
            return LoremIpsum(str.ToString(), Paragraphs);
        }
        #endregion LoremIpsum()

        #region Match()
        /// <summary>
        /// Checks if the current string matches a given search mask.
        /// It ignores duplicate '*' in the mask.  '*' is matched against
        /// 0 or more characters.  Duplicate '?' is treated as requiring
        /// the number of characters.  '?' is matched against 1 or more
        /// characters.
        /// For example:
        ///     "abcdefgh".Match("***a?c*")
        /// will return True while
        ///     "abcdefgh".Match("***ac*")
        /// will return False but
        ///     "abcdefgh".Match("?a?c*")
        /// will also return False because the first '?' requires a character
        /// before 'a'.
        /// </summary>
        /// <param name="str">The current string being matched.</param>
        /// <param name="mask">The search mask being used for the match.</param>
        /// <param name="ignoreCase">Switch to ignore case sensitivity.</param>
        /// <returns>True if the current string matches the given search mask.</returns>
        public static bool Match(this System.String str, 
                                 string mask, 
                                 bool ignoreCase = true)
        {
            ValidateNoNulls(str, mask);
            if (mask == "*")
            {
                return true;
            }
            if (ignoreCase)
            {
                str = str.ToLower();
                mask = mask.ToLower();
            }
            mask = mask.Singularize('*');
            int strOffset = 0;
            int maskOffset = 0;
            bool found = false;
            while (maskOffset < mask.Length)
            {
                switch (mask[maskOffset])
                {
                    case '*':
                        maskOffset++;
                        if (maskOffset >= mask.Length)
                        {
                            return true;
                        }
                        found = false;
                        while (strOffset < str.Length)
                        {
                            if (str[strOffset] != mask[maskOffset])
                            {
                                strOffset++;
                            }
                            else
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            return false;
                        }
                        break;
                    case '?':
                        if (str.ExceedsLength(ref strOffset))
                        {
                            return true;
                        }
                        maskOffset++;
                        break;
                    default:
                        found = false;
                        while (strOffset < str.Length)
                        {
                            if (str[strOffset] != mask[maskOffset])
                            {
                                strOffset++;
                            }
                            else
                            {
                                found = true;
                                break;
                            }
                        }
                        if (found)
                        {
                            maskOffset++;
                            strOffset++;
                        }
                        else
                        {
                            return false;
                        }
                        if (strOffset >= str.Length)
                        {
                            return true;
                        }
                        break;
                }
            }
            return true;
        }

        /// <summary>
        /// Checks if the current string matches a given search mask.
        /// It ignores duplicate '*' in the mask.  '*' is matched against
        /// 0 or more characters.  Duplicate '?' is treated as requiring
        /// the number of characters.  '?' is matched against 1 or more
        /// characters.
        /// For example:
        ///     "abcdefgh".Match("***a?c*")
        /// will return True while
        ///     "abcdefgh".Match("***ac*")
        /// will return False but
        ///     "abcdefgh".Match("?a?c*")
        /// will also return False because the first '?' requires a character
        /// before 'a'.
        /// </summary>
        /// <param name="str">The current string being matched.</param>
        /// <param name="mask">The search mask being used for the match.</param>
        /// <param name="ignoreCase">Switch to ignore case sensitivity.</param>
        /// <returns>True if the current string matches the given search mask.</returns>
        public static bool Match(this System.Text.StringBuilder str,
                                 string mask,
                                 bool ignoreCase = true)
        {
            return Match(str.ToString(), mask, ignoreCase);
        }
        #endregion Match()

        #region MorseCodeBeep()
        /// <summary>
        /// Takes a given System.String representing Morse code and audiblize
        /// it according to standards.
        /// https://www.infoplease.com/encyclopedia/science/engineering/electrical/morse-code
        /// Assumes the input value to be in Morse code format already.
        /// Use .ToMorseCode() to pre-convert text if needed.
        /// </summary>
        /// <param name="str">The System.String text in Morse code format.</param>
        /// <param name="frequency">The beep frequency.</param>
        /// <param name="duration">The duration of a dot beep in ms.</param>
        public static void MorseCodeBeep(this System.String str, 
                                         int frequency = 999, 
                                         int duration = 100)
        {
            Validate(ErrorTypeAll, str, frequency, duration);
            foreach (char c in str)
            {
                switch (c)
                {
                    case '.':
                        Console.Beep(frequency, duration);
                        break;

                    case '-':
                        Console.Beep(frequency, duration * 3);
                        break;

                    case ' ':
                        Console.Beep(frequency, duration * 6);
                        break;
                }
                System.Threading.Thread.Sleep(duration * 3);
            }
        }

        /// <summary>
        /// Takes a given System.Text.StringBuilder representing Morse code
        /// and audiblize it according to standards.
        /// https://www.infoplease.com/encyclopedia/science/engineering/electrical/morse-code
        /// Assumes the input value to be in Morse code format already.
        /// Use .ToMorseCode() to pre-convert text if needed.
        /// </summary>
        /// <param name="str">The System.Text.StringBuilder text in Morse
        /// code format.</param>
        /// <param name="frequency">The beep frequency.</param>
        /// <param name="duration">The duration of a dot beep in ms.</param>
        public static void MorseCodeBeep(this System.Text.StringBuilder str, 
                                         int frequency = 999, 
                                         int duration = 100)
        {
            ValidateNoNulls(str, frequency, duration);
            MorseCodeBeep(str.ToString(), frequency, duration);
        }
        #endregion MorseCodeBeep()

        #region Quote()
        /// <summary>
        /// Return the given string encased in requested quotes.
        /// Default is Constants.QuoteType.Double.
        /// </summary>
        /// <param name="str">The given string to be quoted.</param>
        /// <param name="type">The type of quote to use.  Defaults to double.</param>
        /// <returns>The given string encased in requested quotes.</returns>
        public static string Quote(this System.String str, 
                                   Constants.QuoteType type = 
                                       Constants.QuoteType.Double)
        {
            ValidateNoNulls(str, type);
            if (type == Constants.QuoteType.Double)
            {
                return DoubleQuote(str);
            }
            return SingleQuote(str);
        }

        /// <summary>
        /// Return the given string encased in requested quotes.
        /// Default is Constants.QuoteType.Double.
        /// </summary>
        /// <param name="str">The given string to be quoted.</param>
        /// <param name="type">The type of quote to use.  Defaults to double.</param>
        /// <returns>The given string encased in requested quotes.</returns>
        public static string Quote(this System.Text.StringBuilder str,
                           Constants.QuoteType type =
                               Constants.QuoteType.Double)
        {
            ValidateNoNulls(str, type);
            return Quote(str, type);
        }
        #endregion Quote()

        #region ReplaceTokens()
        /// <summary>
        /// Takes a given string and replaces 1 to n tokens in the string
        /// with replacement tokens as defined in the given Dictionary
        /// of strings.
        /// </summary>
        /// <param name="str">The System.String object upon which token
        /// replacement is to be done.</param>
        /// <param name="tokens">A Dictionary of tokens and replacement
        /// strings to be used for replacement.</param>
        /// <returns>A System.String value with tokens replaced.</returns>
        public static string ReplaceTokens(this System.String str, 
                                           Dictionary<string, string> tokens)
        {
            ValidateNoNulls(str, tokens);
            string returnValue = str;
            foreach (string key in tokens.Keys)
            {
                returnValue = returnValue.Replace(key, tokens[key]);
            }
            return returnValue;
        }

        /// <summary>
        /// Takes a given string and replaces 1 to n tokens in the string
        /// with replacement tokens as defined in the given Dictionary
        /// of strings.
        /// </summary>
        /// <param name="str">The System.Text.StringBuilder upon which
        /// token replacement is to be one.</param>
        /// <param name="tokens">A Dictionary of tokens and replacement
        /// strings to be used for replacement.</param>
        /// <returns>A System.Text.StringBuilder value with tokens replaced.</returns>
        public static string ReplaceTokens(this System.Text.StringBuilder str,
                                           Dictionary<string, string> tokens)
        {
            ValidateNoNulls(str, tokens);
            return ReplaceTokens(str.ToString(), tokens);
        }
        #endregion ReplaceTokens()

        #region RemoveExtraSpace()
        /// <summary>
        /// Trims leading and trailing white space and then removes all extra
        /// white space in the given System.String object returning a single
        /// spaced result.
        /// </summary>
        /// <param name="str">The given System.String object from which
        /// extra spaces needs to be removed.</param>
        /// <returns>The given string object with leading and strailing white
        /// space removed and all other spaces reduced to single space.</returns>
        public static string RemoveExtraSpace(this System.String str)
        {
            ValidateNoNulls(str);
            return System.Text.RegularExpressions.Regex.Replace(str.Trim(), 
                                                                "\\s+", 
                                                                " ");
        }

        /// <summary>
        /// Trims leading and trailing white space and then removes all extra
        /// white space in the given System.Text.StringBuilder returning a 
        /// single spaced result.
        /// </summary>
        /// <param name="str">The given System.Text.StringBuilder object from
        /// which extra spaces needs to be removed.</param>
        /// <returns>The given string object with leading and strailing white
        /// space removed and all other spaces reduced to single space.</returns>
        public static string RemoveExtraSpace(this System.Text.StringBuilder str)
        {
            ValidateNoNulls(str);
            return RemoveExtraSpace(str.ToString());
        }
        #endregion RemoveExtraSpace()

        #region SingleQuote()
        /// <summary>
        /// Return the given string encased in single quotes.
        /// </summary>
        /// <param name="str">The given string to be quoted.</param>
        /// <returns>The given string encased in single quotes.</returns>
        public static string SingleQuote(this System.String str)
        {
            ValidateNoNulls(str);
            return ("'" + str + "'");
        }

        /// <summary>
        /// Return the given string encased in single quotes.
        /// </summary>
        /// <param name="str">The given string to be quoted.</param>
        /// <returns>The given string encased in single quotes.</returns>
        public static string SingleQuote(this System.Text.StringBuilder str)
        {
            ValidateNoNulls(str);
            return SingleQuote(str.ToString());
        }
        #endregion SingleQuote()

        #region Singularize()
        /// <summary>
        /// Parses the given string removing multiples of a given character.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <param name="target">The character to de-duplicate.</param>
        /// <param name="ignoreCase">Switch to ignore case during comparrison.</param>
        /// <returns>The given string with multiples of the given character
        /// removed.</returns>
        public static System.String Singularize(this System.String str,
                                                char target,
                                                bool ignoreCase = true)
        {
            ValidateNoNulls(str, target);
            if (str.Length == 1)
            {
                return str;
            }
            string result = str[0].ToString();
            if (ignoreCase)
            {
                for (int C = 1; C < str.Length; C++)
                {
                    if (str[C].ToString().ToLower()
                        != str[C - 1].ToString().ToLower())
                    {
                        result += str[C];
                    }
                }
            }
            else
            {
                for (int C = 1; C < str.Length; C++)
                {
                    if (str[C]!= str[C - 1])
                    {
                        result += str[C];
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Parses the given string removing multiples of a given character.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <param name="target">The character to de-duplicate.</param>
        /// <param name="ignoreCase">Switch to ignore case during comparrison.</param>
        /// <returns>The given string with multiples of the given character
        /// removed.</returns>
        public static System.String Singularize(this System.Text.StringBuilder str,
                                                char target,
                                                bool ignoreCase = true)
        {
            ValidateNoNulls(str, target);
            return Singularize(str.ToString(), target, ignoreCase);
        }
        #endregion Singularize()

        #region Substring()
        /// <summary>
        /// Extension method to handle FromHead and FromTail types
        /// which allows the caller to return the requested length
        /// of characters from the head or the tail of the given 
        /// string.
        /// </summary>
        /// <param name="str">The given string that is being searched.</param>
        /// <param name="length">The requested length of characters to return.
        /// Use -1 for max.</param>
        /// <param name="type">The type of return string requested.</param>
        /// <returns>FromHead returns the "length" of characters from the head
        /// of the given string.
        /// FromTail returns the "length" of characters from the tail of the
        /// given string.</returns>
        public static string Substring(this System.String str, 
                                       int length,
                                       Constants.SubstringType type = 
                                           Constants.SubstringType.FromHead)
        {
            Validate(ErrorTypeAll, str, length, type);
            if (length == -1)
            {
                length = int.MaxValue;
            }
            switch (type)
            {
                case Constants.SubstringType.FromHead:
                    if (str.Length < length)
                    {
                        return str;
                    }
                    else
                    {
                        return str.Substring(0, length);
                    }
                    break;
                case Constants.SubstringType.FromTail:
                    if (str.Length < length)
                    {
                        return str;
                    }
                    else
                    {
                        return str.Substring(str.Length - length, length);
                    }
                    break;
            }
            return null;
        }

        /// <summary>
        /// Extension method to handle LeftOfIndex and RightOfIndex types
        /// which allows the caller to locate a given number of occurrences
        /// of a given search string and then return the requested length
        /// of characters to either the left or the rigth of the located
        /// search index.
        /// </summary>
        /// <param name="str">The given string that is being searched.</param>
        /// <param name="length">The requested length of characters to return.
        /// Use -1 for max.</param>
        /// <param name="index">The string to search for.</param>
        /// <param name="type">The type of return string requested.</param>
        /// <param name="occurrence">The number of occurrences to match.</param>
        /// <returns>LeftOfIndex returns the "length" of characters to the LEFT
        /// of the located index representing the "occurence"th match of the
        /// "index" string.
        /// RightOfIndex returns the "length" of characters to the RIGHT
        /// of the located index representing the "occurence"th match of the
        /// "index" string.</returns>
        public static string Substring(this System.String str,
                                       int length,
                                       string index,
                                       Constants.SubstringType type =
                                           Constants.SubstringType.LeftOfIndex,
                                       int occurrence = 1)
        {
            Validate(ErrorTypeAll, str, length, index, type, occurrence);
            int offset;
            string temp = str;
            if (length == -1)
            {
                length = int.MaxValue;
            }
            switch (type)
            {
                case Constants.SubstringType.LeftOfIndex:
                    if (occurrence <= 0)
                    {
                        return null;
                    }
                    if (str.IndexOf(index) == -1)
                    {
                        return null;
                    }
                    for (int C = 0; C < occurrence; C++)
                    {
                        temp = temp.Substring(temp.IndexOf(index) + index.Length);
                    }
                    offset = str.IndexOf(temp);
                    temp = str.Substring(0, offset - index.Length);
                    if (temp.Length <= length)
                    {
                        return str.Substring(0, offset - index.Length);
                    }
                    else
                    {
                        return temp.Substring(temp.Length - length);
                    }
                case Constants.SubstringType.RigthOfIndex:
                    if (occurrence <= 0)
                    {
                        return null;
                    }
                    if (str.IndexOf(index) == -1)
                    {
                        return null;
                    }
                    for (int C = 0; C < occurrence; C++)
                    {
                        temp = temp.Substring(temp.IndexOf(index) + index.Length);
                    }
                    if (temp.Length <= length)
                    {
                        return temp;
                    }
                    else
                    {
                        return temp.Substring(0, length);
                    }
                    break;
            }
            return null;
        }

        /// <summary>
        /// Extension method to handle FromHead and FromTail types
        /// which allows the caller to return the requested length
        /// of characters from the head of the given string.
        /// </summary>
        /// <param name="str">The given string that is being searched.</param>
        /// <param name="length">The requested length of characters to return.
        /// Use -1 for max.</param>
        /// <param name="type">The type of return string requested.</param>
        /// <returns>FromHead returns the "length" of characters from the head
        /// of the given string.
        /// FromTail returns the "length" of characters from the tail of the
        /// given string.</returns>
        public static string Substring(this System.Text.StringBuilder str,
                                       int length,
                                       Constants.SubstringType type =
                                           Constants.SubstringType.FromHead)
        {
            Validate(ErrorTypeAll, str, length, type);
            return Substring(str.ToString(),
                             length,
                             type);
        }

        /// <summary>
        /// Extension method to handle LeftOfIndex and RightOfIndex types
        /// which allows the caller to locate a given number of occurrences
        /// of a given search string and then return the requested length
        /// of characters to either the left or the rigth of the located
        /// search index.
        /// </summary>
        /// <param name="str">The given string that is being searched.</param>
        /// <param name="length">The requested length of characters to return.
        /// Use -1 for max.</param>
        /// <param name="index">The string to search for.</param>
        /// <param name="type">The type of return string requested.</param>
        /// <param name="occurrence">The number of occurrences to match.</param>
        /// <returns>LeftOfIndex returns the "length" of characters to the LEFT
        /// of the located index representing the "occurence"th match of the
        /// "index" string.
        /// RightOfIndex returns the "length" of characters to the RIGHT
        /// of the located index representing the "occurence"th match of the
        /// "index" string.</returns>
        public static string Substring(this System.Text.StringBuilder str,
                                       int length,
                                       string index,
                                       Constants.SubstringType type =
                                           Constants.SubstringType.LeftOfIndex,
                                       int occurrence = 1)
        {
            Validate(ErrorTypeAll, str, length, index, type, occurrence);
            return Substring(str.ToString(), 
                             length,
                             index,
                             type,
                             occurrence);
        }

        /// <summary>
        /// Retrieves a substring from this instance. The substring starts 
        /// at a specified character position and continues to the end of the
        /// string.
        /// </summary>
        /// <param name="str">The StringBuilder source instance.</param>
        /// <param name="startIndex">The starting position of the substring
        /// selection.</param>
        /// <returns>A string that is equivalent to the substring that begins
        /// at startIndex in this instance, or System.String.Empty if
        /// startIndex is equal to the length of this instance.</returns>
        public static string Substring(this System.Text.StringBuilder str,
                                       int startIndex)
        {
            Validate(ErrorTypeAll, str, startIndex);
            return str.ToString().Substring(startIndex);
        }

        /// <summary>
        /// Retrieves a substring from this instance. The substring starts at
        /// a specified character position and has a specified length.
        /// </summary>
        /// <param name="str">The StringBuilder source instance.</param>
        /// <param name="startIndex">The starting position of the substring
        /// selection.</param>
        /// <param name="length">The length of the substring to be selected.</param>
        /// <returns>A string that is equivalent to the substring of length
        /// length that begins at startIndex in this instance, or 
        /// System.String.Empty if startIndex is equal to the length of this
        /// instance and length is zero.</returns>
        public static string Substring(this System.Text.StringBuilder str,
                                       int startIndex,
                                       int length)
        {
            Validate(ErrorTypeAll, str, startIndex, length);
            return str.ToString().Substring(startIndex, length);
        }
        #endregion Substring()

        #region ToBinary()
        /// <summary>
        /// Returns the binary representation of a given string object.
        /// </summary>
        /// <param name="str">The System.String object to convert to binary.</param>
        /// <returns></returns>
        public static string ToBinary(this System.String str)
        {
            ValidateNoNulls(str);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (byte b in System.Text.ASCIIEncoding.UTF8.GetBytes(
                str.ToCharArray()))
            {
                sb.Append(Convert.ToString(b, 2) + " ");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Returns the binary representation of a given StringBuilder object.
        /// </summary>
        /// <param name="str">The System.Text.StringBuilder object to convert
        /// to binary.</param>
        /// <returns></returns>
        public static string ToBinary(this System.Text.StringBuilder str)
        {
            ValidateNoNulls(str);
            return ToBinary(str.ToString());
        }
        #endregion ToBinary()

        #region ToEnum()
        /// <summary>
        /// Convert a System.String to its Enum value.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="str">The string to match to an enum.</param>
        /// <returns>The enum value.</returns>
        public static T ToEnum<T>(this System.String str)
        {
            ValidateNoNulls(str);
            return (T)Enum.Parse(typeof(T), str);
        }

        /// <summary>
        /// Convert a System.String to its Enum value.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="str">The string to match to an enum.</param>
        /// <returns>The enum value.</returns>
        public static T ToEnum<T>(this System.Text.StringBuilder str)
        {
            ValidateNoNulls(str);
            return (str.ToString().ToEnum<T>());
        }
        #endregion ToEnum()

        #region ToEnumerable
        /// <summary>
        /// Converts the given querystring to a Dictionary.
        /// </summary>
        /// <param name="str">The given querystring to convert.</param>
        /// <param name="separator">Defaults to ampersand per W3C standards.</param>
        /// <param name="assigner">Defaults to = per W3C standards.</param>
        /// <returns>The parsed dictionary containing querystring values.</returns>
        public static Dictionary<string, string> QueryStringToDictionary(
            this System.String str,
            char separator = '&',
            char assigner = '=')
        {
            ValidateNoNulls(str, separator, assigner);
            Dictionary<string, string> result = new Dictionary<string, string>();
            string query = str.Substring(str.IndexOf('?') + 1);
            string[] parts = query.Split(separator);
            foreach (string part in parts)
            {
                string[] pair = part.Split(assigner);
                result.Add(pair[0], pair[1]);
            }
            return result;
        }

        /// <summary>
        /// Converts the given querystring to a Dictionary.
        /// </summary>
        /// <param name="str">The given querystring to convert.</param>
        /// <param name="separator">Defaults to ampersand per W3C standards.</param>
        /// <param name="assigner">Defaults to = per W3C standards.</param>
        /// <returns>The parsed dictionary containing querystring values.</returns>
        public static Dictionary<string, string> QueryStringToDictionary(
            this System.Text.StringBuilder str,
            char separator = '&',
            char assigner = '=')
        {
            ValidateNoNulls(str, separator, assigner);
            return QueryStringToDictionary(str.ToString(), separator, assigner);
        }

        /// <summary>
        /// Converts the given querystring to a NameValueCollection.
        /// </summary>
        /// <param name="str">The given querystring to convert.</param>
        /// <param name="separator">Defaults to ampersand per W3C standards.</param>
        /// <param name="assigner">Defaults to = per W3C standards.</param>
        /// <returns>The parsed NameValueCollection containing querystring
        /// values.</returns>
        public static System.Collections.Specialized.NameValueCollection QueryStringToNameValueCollection(
            this System.String str,
            char separator = '&',
            char assigner = '=')
        {
            ValidateNoNulls(str, separator, assigner);
            System.Collections.Specialized.NameValueCollection nvc = 
                new System.Collections.Specialized.NameValueCollection();
            foreach (KeyValuePair<string, string> kvp in QueryStringToDictionary(str))
            {
                nvc.Add(kvp.Key, kvp.Value);
            }
            return nvc;
        }

        /// <summary>
        /// Converts the given querystring to a NameValueCollection.
        /// </summary>
        /// <param name="str">The given querystring to convert.</param>
        /// <param name="separator">Defaults to ampersand per W3C standards.</param>
        /// <param name="assigner">Defaults to = per W3C standards.</param>
        /// <returns>The parsed NameValueCollection containing querystring
        /// values.</returns>
        public static System.Collections.Specialized.NameValueCollection QueryStringToNameValueCollection(
            this System.Text.StringBuilder str,
            char separator = '&',
            char assigner = '=')
        {
            ValidateNoNulls(str, separator, assigner);
            return QueryStringToNameValueCollection(str.ToString(), 
                                                    separator, 
                                                    assigner);
        }
        #endregion ToEnumerable

        #region ToMorseCode()
        /// <summary>
        /// Convert given System.String to its Morse code representation.
        /// Undefined characters will return in the format:
        /// Undefined:char
        /// For example:
        /// "sos@".ToMorseCode()
        /// will return
        /// "...---...Undefined:@"
        /// </summary>
        /// <param name="str">The given string to convert to Morse code.</param>
        /// <param name="includeSpaces">Are spaces included in translation.</param>
        /// <returns>The Morse code represenation of the given string.</returns>
        public static string ToMorseCode(this System.String str, 
                                         bool includeSpaces = true)
        {
            ValidateNoNulls(str, includeSpaces);
            System.Text.StringBuilder returnValue = 
                new System.Text.StringBuilder();
            foreach (char c in str.ToLower())
            {
                if (Constants.MorseCode.TryGetValue(c, out string tempStr))
                {
                    returnValue.Append(c != ' ' ? tempStr : 
                        (includeSpaces ? tempStr : ""));
                }
                else
                {
                    returnValue.Append("<Undefined:[" + c + "]>");
                }
            }
            return returnValue.ToString();
        }

        /// <summary>
        /// Convert given System.Text.Stringbuilder object to its Morse code
        /// representation.
        /// Undefined characters will return in the format:
        /// Undefined:char>
        /// For example:
        /// "sos@".ToMorseCode()
        /// will return
        /// "...---...Undefined:@"
        /// </summary>
        /// <param name="str">The given string to convert to Morse code.</param>
        /// <param name="includeSpaces">Are spaces included in translation.</param>
        /// <returns>The Morse code represenation of the given string.</returns>
        public static string ToMorseCode(this System.Text.StringBuilder str, 
                                         bool includeSpaces = true)
        {
            ValidateNoNulls(str, includeSpaces);
            return (str.ToString());
        }
        #endregion ToMorseCode()

        #region TrimLength()
        /// <summary>
        /// Returns part of the given System.String object tuncated to 
        /// the requested length minus the length of the suffix.
        /// If the string is null or empty, it returns said value.
        /// If the string is shorter than the requested length, it returns
        /// the whole string.
        /// </summary>
        /// <param name="str">The given System.String object.</param>
        /// <param name="length">The requested length of the return string.</param>
        /// <param name="suffix">The string appended to the end of the
        /// returned string.  Default value is "..."</param>
        /// <returns>Returns part of the given System.String object tuncated 
        /// to the requested length minus the length of the suffix.</returns>
        public static string TrimLength(this System.String str, 
                                        int length, 
                                        string suffix = "...")
        {
            ValidateNoNulls(str, length, suffix);
            return (string.IsNullOrEmpty(str) || str.Length < length ? str :
                (str.Substring(0, length - suffix.Length) + suffix));
        }

        /// <summary>
        /// Returns part of the given System.Text.StringBuilder object 
        /// tuncated to the requested length minus the length of the 
        /// suffix.
        /// If the string is null or empty, it returns said value.
        /// If the string is shorter than the requested length, it returns
        /// the whole string.
        /// </summary>
        /// <param name="str">The given System.Text.StringBuilder object.</param>
        /// <param name="length">The requested length of the return string.</param>
        /// <param name="suffix">The string appended to the end of the
        /// returned string.  Default value is "..."</param>
        /// <returns>Returns part of the given System.String object tuncated 
        /// to the requested length minus the length of the suffix.</returns>
        public static string TrimLength(this System.Text.StringBuilder str, 
                                        int length,
                                        string suffix = "...")
        {
            ValidateNoNulls(str, length, suffix);
            return TrimLength(str.ToString(), length, suffix);
        }
        #endregion TrimLength()

        #region TrimStart()
        /// <summary>
        /// Trims the starting target string from the current string.
        /// </summary>
        /// <param name="str">The current string to be trimmed.</param>
        /// <param name="target">The target string to trim off the current
        /// string.</param>
        /// <returns>The current string minus the target string IF the current
        /// string begins with the target string, else it returns the current
        /// string.</returns>
        public static string TrimStart(this System.String str,
                                       string target)
        {
            ValidateNoNulls(str, target);
            if (str.BeginsWith(target))
            {
                return str.Substring(target.Length);
            }
            return str;
        }
        #endregion TrimStart()

        #region Words()
        /// <summary>
        /// Returns the number of words in the given string object.
        /// </summary>
        /// <param name="str">A System.String object for which to count 
        /// words.</param>
        /// <returns>The number of words in the given object.</returns>
        public static int Words(this System.String str)
        {
            ValidateNoNulls(str);
            return str.Split(new char[] { ' ',
                                          '.',
                                          '?',
                                          '!',
                                          ';' },
                             StringSplitOptions.RemoveEmptyEntries)
                      .Length;
        }

        /// <summary>
        /// Returns the number of words in the given string builder object.
        /// </summary>
        /// <param name="str">A System.Text.StringBuilder object for which
        /// to count words.</param>
        /// <returns>The number of words in the given object.</returns>
        public static int Words(this System.Text.StringBuilder str)
        {
            ValidateNoNulls(str);
            return Words(str.ToString());
        }
        #endregion Words()

        //public static bool ToEmailSafeTextFile(this System.String filePath)
        //{
        //    if (System.IO.File.Exists(filePath))
        //    {
        //        System.IO.File.WriteAllText(
        //            filePath + ".txt",
        //            Convert.ToBase64String(
        //                System.IO.File.ReadAllBytes(filePath)));
        //        return true;
        //    }
        //    return false;
        //}

        //public static bool FromEmailSafeTextFile(this System.String filePath)
        //{
        //    if (System.IO.File.Exists(filePath))
        //    {
        //        System.IO.File.WriteAllBytes(
        //            filePath.ToLower()
        //                    .TrimEnd(new char[] { 
        //                        '.', 
        //                        't', 
        //                        'x', 
        //                        't'}),
        //            Convert.FromBase64String(
        //                System.IO.File.ReadAllText(filePath)));
        //        return true;
        //    }
        //    return false;
        //}
    }
}
#pragma warning restore CS1587, CS0162
