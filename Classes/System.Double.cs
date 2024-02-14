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
    /// Extension methods for the System.Double class.
    /// </summary>
    [Serializable]
    public static partial class DoubleExtensions
    {
        #region BinaryDataSizes
        /// <summary>
        /// Returns the given number expressed as Bytes.
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Bytes.</returns>
        public static double ToNumberBytes(this double number, 
                                           Constants.NumberType fromType = 
                                               Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            switch (fromType)
            {
                case Constants.NumberType.Bytes:
                    return number;

                case Constants.NumberType.KB:
                    return number * Constants.KB;

                case Constants.NumberType.MB:
                    return number * Constants.MB;

                case Constants.NumberType.GB:
                    return number * Constants.GB;

                case Constants.NumberType.TB:
                    return number * Constants.TB;

                case Constants.NumberType.PB:
                    return number * Constants.PB;

                case Constants.NumberType.EB:
                    return number * Constants.EB;

                case Constants.NumberType.ZB:
                    return number * Constants.ZB;

                case Constants.NumberType.YB:
                    return number * Constants.YB;

                case Constants.NumberType.BB:
                    return number * Constants.BB;

                case Constants.NumberType.GpB:
                    return number * Constants.GpB;

                case Constants.NumberType.SB:
                    return number * Constants.SB;

                case Constants.NumberType.PaB:
                    return number * Constants.PaB;

                case Constants.NumberType.AB:
                    return number * Constants.AB;

                case Constants.NumberType.PlB:
                    return number * Constants.PlB;

                case Constants.NumberType.BrB:
                    return number * Constants.BrB;

                case Constants.NumberType.SoB:
                    return number * Constants.SoB;

                case Constants.NumberType.QB:
                    return number * Constants.QB;

                case Constants.NumberType.KaB:
                    return number * Constants.KaB;

                case Constants.NumberType.RB:
                    return number * Constants.RB;

                case Constants.NumberType.DB:
                    return number * Constants.DB;

                case Constants.NumberType.HB:
                    return number * Constants.HB;

                case Constants.NumberType.MrB:
                    return number * Constants.MrB;

                case Constants.NumberType.DdB:
                    return number * Constants.DdB;

                case Constants.NumberType.RtB:
                    return number * Constants.RtB;

                case Constants.NumberType.ShB:
                    return number * Constants.ShB;

                case Constants.NumberType.CB:
                    return number * Constants.CB;

                case Constants.NumberType.KkB:
                    return number * Constants.KkB;
            }
            return number;
        }

        /// <summary>
        /// Returns the given number expressed as Kilobytes (2^10).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Kilobytes.</returns>
        public static double ToKB(this double number,
                                  Constants.NumberType fromType =
                                      Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.KB;
        }

        /// <summary>
        /// Returns the given number expressed as Megabytes (2^20).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Megabytes.</returns>
        public static double ToMB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.MB;
        }

        /// <summary>
        /// Returns the given number expressed as Gigabytes (2^30).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Gigabytes.</returns>
        public static double ToGB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.GB;
        }

        /// <summary>
        /// Returns the given number expressed as Terrabytes (2^40).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Terrabytes.</returns>
        public static double ToTB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.TB;
        }

        /// <summary>
        /// Returns the given number expressed as Petabytes (2^50).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Petabytes.</returns>
        public static double ToPB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.PB;
        }

        /// <summary>
        /// Returns the given number expressed as Exabytes (2^60).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Exabytes.</returns>
        public static double ToEB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.EB;
        }

        /// <summary>
        /// Returns the given number expressed as Zettabytes (2^70).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Zettabytes.</returns>
        public static double ToZB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.ZB;
        }

        /// <summary>
        /// Returns the given number expressed as Yottabytes (2^80).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Yottabytes.</returns>
        public static double ToYB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.YB;
        }

        /// <summary>
        /// Returns the given number expressed as Brontobytes (2^90).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Brontobytes.</returns>
        public static double ToBB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.BB;
        }

        /// <summary>
        /// Returns the given number expressed as Geopbytes (2^100).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Geopbytes.</returns>
        public static double ToGpB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.GpB;
        }

        /// <summary>
        /// Returns the given number expressed as Saganbytes (2^110).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Saganbytes.</returns>
        public static double ToSB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.SB;
        }

        /// <summary>
        /// Returns the given number expressed as Pijabytes (2^120).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Pijabytes.</returns>
        public static double ToPaB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.PaB;
        }

        /// <summary>
        /// Returns the given number expressed as Alphabytes (2^130).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Alphabytes.</returns>
        public static double ToAB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.AB;
        }

        /// <summary>
        /// Returns the given number expressed as Pectrolbytes (2^140).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Pectrolbytes.</returns>
        public static double ToPlB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.PlB;
        }

        /// <summary>
        /// Returns the given number expressed as Bolgerbytes (2^150).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Bolgerbytes.</returns>
        public static double ToBrB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.BrB;
        }

        /// <summary>
        /// Returns the given number expressed as Sambobytes (2^160).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Sambobytes.</returns>
        public static double ToSoB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.SoB;
        }

        /// <summary>
        /// Returns the given number expressed as Quesabytes (2^170).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Quesabytes.</returns>
        public static double ToQB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.QB;
        }

        /// <summary>
        /// Returns the given number expressed as Kinsabytes (2^180).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Kinsabytes.</returns>
        public static double ToKaB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.KaB;
        }

        /// <summary>
        /// Returns the given number expressed as Rutherbytes (2^190).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Rutherbytes.</returns>
        public static double ToRB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.RB;
        }

        /// <summary>
        /// Returns the given number expressed as Dubnibytes (2^200).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Dubnibytes.</returns>
        public static double ToDB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.DB;
        }

        /// <summary>
        /// Returns the given number expressed as Hassiubytes (2^210).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Hassiubytes.</returns>
        public static double ToHB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.HB;
        }

        /// <summary>
        /// Returns the given number expressed as Meitnerbytes (2^220).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Meitnerbytes.</returns>
        public static double ToMrB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.MrB;
        }

        /// <summary>
        /// Returns the given number expressed as Darmstadbytes (2^230).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Darmstadbytes.</returns>
        public static double ToDdB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.DdB;
        }

        /// <summary>
        /// Returns the given number expressed as Roentbytes (2^240).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Roentbytes.</returns>
        public static double ToRtB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.RtB;
        }

        /// <summary>
        /// Returns the given number expressed as Sophobytes (2^250).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Sophobytes.</returns>
        public static double ToShB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.ShB;
        }

        /// <summary>
        /// Returns the given number expressed as Coperbytes (2^260).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Coperbytes.</returns>
        public static double ToCB(this double number,
                          Constants.NumberType fromType =
                              Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.CB;
        }

        /// <summary>
        /// Returns the given number expressed as Koentekbytes (2^270).
        /// </summary>
        /// <param name="number">The given number to convert.</param>
        /// <param name="fromType">The number type of the given number.</param>
        /// <returns>Returns the given number in Koentekbytes.</returns>
        public static double ToKkB(this double number,
                                   Constants.NumberType fromType =
                                       Constants.NumberType.Bytes)
        {
            ValidateNoNulls(number, fromType);
            return ToNumberBytes(number, fromType) / Constants.KkB;
        }
        #endregion BinaryDataSizes

        #region CompoundInterest()
        /// <summary>
        /// Calculate compounded interest end value given an amount, percent
        /// interest per year and number of years.
        /// </summary>
        /// <param name="amount">The starting amount.</param>
        /// <param name="percent">The annual interest rate 
        /// i.e. 4% = 4 not 0.04</param>
        /// <param name="years">The years of interest to calculate.</param>
        /// <param name="frequency">The frequency with which interest is
        /// applied.</param>
        /// <returns>The compounded value.</returns>
        public static double CompoundInterest(this double amount, 
                                              double percent = 0, 
                                              int years = 0, 
                                              Constants.CompoundFrequency frequency = 
                                                  Constants.CompoundFrequency.Monthly)
        {
            Validate(Constants.ErrorTypeAll, amount, percent, years, frequency);
            double result = amount;
            double Percent = percent / 100 / (
                frequency == Constants.CompoundFrequency.Yearly ? 1 : 12);
            int Years = years * (
                frequency == Constants.CompoundFrequency.Yearly ? 1 : 12);
            for (int C = 0; C < Years; C++)
            {
                result += (result * Percent);
            }
            return result;
        }
        #endregion CompoundInterest()

        #region Convert.To Overloaded Methods
        /// <summary>
        /// Shorthand extension encapsulating Convert.ToInt16() allowing
        /// syntax changes from this:
        /// ...Convert.ToInt16(doubleValue)...
        /// to:
        /// ...doubleValue.ToInt16()...
        /// </summary>
        /// <param name="number">The double value to convert.</param>
        /// <returns>The value as Int16 format.</returns>
        public static Int16 ToInt16(this double number)
        {
            ValidateNoNulls(number);
            return Convert.ToInt16(number);
        }
        #endregion Convert.To Overloaded Methods

        #region IsEven()
        /// <summary>
        /// Checks if the given number is even.
        /// </summary>
        /// <param name="number">The given number to check.</param>
        /// <returns>True if the number is even, else False.</returns>
        public static bool IsEven(this double number)
        {
            ValidateNoNulls(number);
            return (number % 2 == 0);
        }
        #endregion IsEven()

        #region IsOdd()
        /// <summary>
        /// Checks if the given number is odd.
        /// </summary>
        /// <param name="number">The given number to check.</param>
        /// <returns>True if the number is odd, else False.</returns>
        public static bool IsOdd(this double number)
        {
            ValidateNoNulls(number);
            return (number % 2 != 0);
        }
        #endregion IsOdd()

        #region IsPrime()
        /// <summary>
        /// Checks if the given number is a prime number.
        /// </summary>
        /// <param name="number">The given number to evaluate as a prime.</param>
        /// <returns>True if the given number is prime, else False.</returns>
        public static bool IsPrime(this double number)
        {
            return UInt64Extensions.IsPrime((System.UInt64)number, true);
        }
        #endregion IsPrime()
    }
}
