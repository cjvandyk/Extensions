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
    public static class Double
    {
        public static void Test()
        {
            printf("********* Double Testing *********", ConsoleColor.Green);
            double val = 123.456;
            printf($"{val} compounded monthly over 10 years at 4.5% per annum:");
            printf(
                val.CompoundInterest(
                    4.5,
                    10,
                    Constants.CompoundFrequency.Monthly));
            val = 100.00;
            printf($"{val} compounded annually over 10 years at 5% per annum:");
            printf(
                val.CompoundInterest(
                    5,
                    10,
                    Constants.CompoundFrequency.Yearly));
            printf($"{val} compounded monthly over 10 years at 5% per annum:");
            printf(
                val.CompoundInterest(
                    5,
                    10,
                    Constants.CompoundFrequency.Monthly));
            val = 100000.00;
            printf(
                val.CompoundInterest(
                    0.018,
                    30,
                    Constants.CompoundFrequency.Monthly));
            printf(
                val.CompoundInterest(
                    0.041,
                    30,
                    Constants.CompoundFrequency.Monthly));
            double dbl = 1;
            printf(dbl.ToKB(Constants.NumberType.TB));
            printf(dbl.ToKB(Constants.NumberType.GB));
            printf(dbl.ToKB(Constants.NumberType.ZB));
        }
    }
}
