/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;

using Extensions;
using static Extensions.Core;

namespace TESTING
{
    public static class String
    {
        enum e { abc, xyz, qwe, rty };

        public static void Test()
        {
            printf("********* String Testing *********", ConsoleColor.Green);
            string s;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //Test System.String.Substring()
            printf("System.String.Substring()", ConsoleColor.Yellow);
            s = "abc1abc2abc3abc4";
            printf(s.Substring(3, Constants.SubstringType.FromHead));
            printf(s.Substring(5, Constants.SubstringType.FromHead));
            printf(s.Substring(8, Constants.SubstringType.FromHead));
            printf(s.Substring(3, Constants.SubstringType.FromTail));
            printf(s.Substring(5, Constants.SubstringType.FromTail));
            printf(s.Substring(8, Constants.SubstringType.FromTail));
            printf(s.Substring(5, "abc", Constants.SubstringType.LeftOfIndex, 0));
            printf(s.Substring(5, "abc", Constants.SubstringType.LeftOfIndex, 1));
            printf(s.Substring(5, "abc", Constants.SubstringType.LeftOfIndex, 2));
            printf(s.Substring(5, "abc", Constants.SubstringType.LeftOfIndex, 3));
            printf(s.Substring(5, "abc", Constants.SubstringType.LeftOfIndex, 4));
            printf(s.Substring(5, "abc", Constants.SubstringType.RigthOfIndex, 0));
            printf(s.Substring(5, "abc", Constants.SubstringType.RigthOfIndex, 1));
            printf(s.Substring(5, "abc", Constants.SubstringType.RigthOfIndex, 2));
            printf(s.Substring(5, "abc", Constants.SubstringType.RigthOfIndex, 3));
            printf(s.Substring(5, "abc", Constants.SubstringType.RigthOfIndex, 4));
            printf("System.Text.StringBuilder.Substring()", ConsoleColor.Yellow);
            sb.Append(s);
            printf(sb.Substring(5));
            printf(sb.Substring(5, 3));
            printf(sb.Substring(3, Constants.SubstringType.FromHead));
            printf(sb.Substring(5, Constants.SubstringType.FromHead));
            printf(sb.Substring(8, Constants.SubstringType.FromHead));
            printf(sb.Substring(3, Constants.SubstringType.FromTail));
            printf(sb.Substring(5, Constants.SubstringType.FromTail));
            printf(sb.Substring(8, Constants.SubstringType.FromTail));
            printf(sb.Substring(5, "abc", Constants.SubstringType.LeftOfIndex, 0));
            printf(sb.Substring(5, "abc", Constants.SubstringType.LeftOfIndex, 1));
            printf(sb.Substring(5, "abc", Constants.SubstringType.LeftOfIndex, 2));
            printf(sb.Substring(5, "abc", Constants.SubstringType.LeftOfIndex, 3));
            printf(sb.Substring(5, "abc", Constants.SubstringType.LeftOfIndex, 4));
            printf(sb.Substring(5, "abc", Constants.SubstringType.RigthOfIndex, 0));
            printf(sb.Substring(5, "abc", Constants.SubstringType.RigthOfIndex, 1));
            printf(sb.Substring(5, "abc", Constants.SubstringType.RigthOfIndex, 2));
            printf(sb.Substring(5, "abc", Constants.SubstringType.RigthOfIndex, 3));
            printf(sb.Substring(5, "abc", Constants.SubstringType.RigthOfIndex, 4));

            //Test System.String.DoubleQuote()
            //     System.String.SingleQuote()
            //     System.String.Quote()
            printf(".DoubleQuote()", ConsoleColor.Yellow);
            s = "https://blog.cjvandyk.com/sites/Rocks";
            printf(s);
            //printf(s.DoubleQuote());
            printf(".SingleQuote()", ConsoleColor.Yellow);
            //printf(s.SingleQuote());
            printf(".Quote()", ConsoleColor.Yellow);
            //printf(s.Quote());

            //Test System.String.GetUrlRoot()
            printf(".GetUrlRoot()", ConsoleColor.Yellow);
            s = "https://blog.cjvandyk.com/sites/Rocks";
            printf(s);
            printf(s.GetUrlRoot());

            //Test System.String.HasLower()
            printf(".HasLower()", ConsoleColor.Yellow);
            s = "abc";
            printf(s);
            printf(s.HasLower());
            s = "ABC";
            printf(s);
            printf(s.HasLower());

            //Test System.String.HasUpper()
            printf(".HasUpper()", ConsoleColor.Yellow);
            s = "abc";
            printf(s);
            printf(s.HasUpper());
            s = "ABC";
            printf(s);
            printf(s.HasUpper());

            //Test System.String.HasNumeric()
            printf(".HasNumeric()", ConsoleColor.Yellow);
            s = "abc";
            printf(s);
            printf(s.HasNumeric());
            s = "ABC123";
            printf(s);
            printf(s.HasNumeric());

            //Test System.String.HasSymbol()
            printf(".HasSymbol()", ConsoleColor.Yellow);
            s = "abc";
            printf(s);
            printf(s.HasSymbol());
            s = "ABC@#";
            printf(s);
            printf(s.HasSymbol());

            //Test System.String.HtmlDecode()
            //     System.String.HtmlEncode()
            printf(".HtmlEncode()", ConsoleColor.Yellow);
            s = "https://blog.cjvandyk.com/sites/Rocks <&> Rolls!";
            printf(s);
            //s = s.HtmlEncode();
            printf(s);
            printf(".HtmlDecode()", ConsoleColor.Yellow);
            //printf(s.HtmlDecode());

            //Test System.String.IsAlphabetic()
            printf(".IsAlphabetic()", ConsoleColor.Yellow);
            s = "abc";
            printf(s);
            printf(s.IsAlphabetic());
            s = "ABC@#";
            printf(s);
            printf(s.IsAlphabetic());

            //Test System.String.IsAlphaNumeric()
            printf(".IsAlphaNumeric()", ConsoleColor.Yellow);
            s = "abc123";
            printf(s);
            printf(s.IsAlphaNumeric());
            s = "ABC@#";
            printf(s);
            printf(s.IsAlphaNumeric());

            //Test System.String.IsChar()
            printf(".IsChar()", ConsoleColor.Yellow);
            s = "abc123";
            printf(s);
            printf(s.IsChar(new char[] { '1', '2' }));
            s = "ABC@#";
            printf(s);
            printf(s.IsChar(new char[] { '1', '2' }));

            //Test System.String.IsEmail()
            printf(".IsEmail()", ConsoleColor.Yellow);
            s = "bigboss@cjvandyk.com";
            printf(s);
            printf(s.IsEmail());
            s = "bigbossATcjvandyk.com";
            printf(s);
            printf(s.IsEmail());

            //Test System.String.IsLower()
            printf(".IsLower()", ConsoleColor.Yellow);
            s = "abc";
            printf(s);
            printf(s.IsLower());
            s = "ABC";
            printf(s);
            printf(s.IsLower());

            //Test System.String.IsNumeric()
            printf(".IsNumeric()", ConsoleColor.Yellow);
            s = "abc";
            printf(s);
            printf(s.IsNumeric());
            s = "123";
            printf(s);
            printf(s.IsNumeric());

            //Test System.String.IsStrong()
            printf(".IsStrong()", ConsoleColor.Yellow);
            s = "abc";
            printf(s);
            printf(s.IsStrong());
            s = "abc";
            printf(s);
            printf(s.IsStrong(1, false, false, false, false));
            s = "abc123";
            printf(s);
            printf(s.IsStrong());
            s = "abc123";
            printf(s);
            printf(s.IsStrong(2, false, false, false, false));
            s = "abc123ABC";
            printf(s);
            printf(s.IsStrong());
            s = "abc123ABC";
            printf(s);
            printf(s.IsStrong(3, false, false, false, false));
            s = "abc123ABC$";
            printf(s);
            printf(s.IsStrong());
            s = "abc123ABC";
            printf(s);
            printf(s.IsStrong(3, true, false, false, false));
            printf(s.IsStrong(3, false, true, false, false));
            printf(s.IsStrong(3, false, false, true, false));
            printf(s.IsStrong(3, false, false, false, true));

            //Test System.String.IsUpper()
            printf(".IsUpper()", ConsoleColor.Yellow);
            s = "abc";
            printf(s);
            printf(s.IsUpper());
            s = "Abc";
            printf(s);
            printf(s.IsUpper());
            s = "ABC";
            printf(s);
            printf(s.IsUpper());

            //Test System.String.IsUrlRoot()
            printf(".IsUrlRoot()", ConsoleColor.Yellow);
            s = "https://blog.cjvandyk.com/sites/Rocks";
            printf(s);
            printf(s.IsUrlRoot());
            printf(s.GetUrlRoot().IsUrlRoot());
            s = "https://blog.cjvandyk.com/";
            printf(s.IsUrlRoot());
            s = "https://blog.cjvandyk.com";
            printf(s.IsUrlRoot());

            //Test System.Char.IsVowel()
            printf(".IsVowel()", ConsoleColor.Yellow);
            s = "https://blog.cjvandyk.com/sites/Rocks";
            printf(s[0]);
            printf(s[0].IsVowel());
            printf(s[10]);
            printf(s[10].IsVowel());

            //Test System.String.IsZipCode()
            printf(".IsZipCode()", ConsoleColor.Yellow);
            s = "030600";
            printf(s);
            printf(s.IsZipCode());
            s = "03060";
            printf(s);
            printf(s.IsZipCode());
            s = "030601234";
            printf(s);
            printf(s.IsZipCode());
            s = "03060-1234";
            printf(s);
            printf(s.IsZipCode());

            //Test System.String.Lines()
            printf(".Lines()", ConsoleColor.Yellow);
            s = s.LoremIpsum(1);
            printf(s);
            printf(s.Lines());
            s = s.LoremIpsum(3);
            printf(s);
            printf(s.Lines());

            //Test System.String.LoremIpsum()
            printf(".LoremIpsum()", ConsoleColor.Yellow);
            printf(s.LoremIpsum(2));

            //Test System.String.MorseCodeBeep()
            printf(".MorseCodeBeep()", ConsoleColor.Yellow);
            s = "SOS";
            printf(s);
            s.ToMorseCode().MorseCodeBeep(999, 50);

            //Test System.String.ReplaceTokens()
            printf(".ReplaceTokens()", ConsoleColor.Yellow);
            s = "abc123";
            System.Collections.Generic.Dictionary<string, string> dic = 
                new System.Collections.Generic.Dictionary<string, string>();
            dic.Add("1", "X");
            dic.Add("3", "Y");
            dic.Add("b", "Z");
            printf(s);
            printf(s.ReplaceTokens(dic));

            //Test System.String.RemoveExtraSpace()
            printf(".RemoveExtraSpace()", ConsoleColor.Yellow);
            s = "   blog.cjvandyk.com       ROCKS!       ";
            printf(s);
            printf(s.RemoveExtraSpace());

            //Test System.String.ToBinary()
            printf(".ToBinary()", ConsoleColor.Yellow);
            s = "blog.cjvandyk.com ROCKS!";
            printf(s);
            printf(s.ToBinary());

            //Test System.String.ToEnum()
            printf(".ToEnum()", ConsoleColor.Yellow);
            printf(e.abc);
            s = "xyz";
            printf(s);
            var testEnumResult = s.ToEnum<e>();
            printf(testEnumResult);
            printf(testEnumResult == e.xyz);

            //Test System.Collections.Specialed.NameValueCollection.QueryStringToNameValueCollection()
            printf(".ToEnumerable()", ConsoleColor.Yellow);
            printf(".QueryStringToNameValueCollection()", ConsoleColor.Cyan);
            System.Collections.Specialized.NameValueCollection nvc =
                "https://blog.cjvandyk.com/sites/Test?qs1=One&qs2=Two".QueryStringToNameValueCollection();
            foreach (string key in nvc.Keys)
            {
                printf(key);
                printf(nvc[key]);
            }

            //Test System.String.QueryStringToDictionary()
            printf(".QueryStringToDictionary()", ConsoleColor.Cyan);
            dic = "https://blog.cjvandyk.com/sites/Test?qs1=One&qs2=Two".QueryStringToDictionary();
            foreach (string key in dic.Keys)
            {
                printf(key);
                printf(dic[key]);
            }

            //Test System.String.ToMorseCode()
            printf(".ToMorseCode()", ConsoleColor.Yellow);
            s = "SOS";
            printf(s);
            printf(s.ToMorseCode());

            //Test System.String.TrimLength()
            printf(".TrimLength()", ConsoleColor.Yellow);
            s = s.LoremIpsum(1);
            printf(s);
            printf(s.TrimLength(80));
            printf(s.TrimLength(80, "yada, yada, yada"));

            //Test System.String.Words()
            printf(".Words()", ConsoleColor.Yellow);
            s = s.LoremIpsum(1);
            printf(s);
            printf(s.Words());
        }
    }
}
