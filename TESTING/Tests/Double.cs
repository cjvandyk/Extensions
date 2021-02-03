#pragma warning disable IDE1006, IDE0017, CS0162, IDE0060 // Naming Styles, Simplify declaration (FQCN used), break after return, Remove unused (string[] args)

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Quix/blob/master/LICENSE
/// </summary>

using System;

using Extensions;

namespace TESTING
{
    public static class Double
    {
        public static void Test()
        {
            double val = 123.456;
            Universal.printf($"{val} compounded monthly over 10 years at 4.5% per annum:");
            Universal.printf(Convert.ToString(
                val.CompoundInterest(4.5, 
                                     10, 
                                     Constants.CompoundFrequency.Monthly)));
            val = 100.00;
            Universal.printf($"{val} compounded annually over 10 years at 5% per annum:");
            Universal.printf(Convert.ToString(
                val.CompoundInterest(5,
                                     10,
                                     Constants.CompoundFrequency.Yearly)));
            Universal.printf($"{val} compounded monthly over 10 years at 5% per annum:");
            Universal.printf(Convert.ToString(
                val.CompoundInterest(5,
                                     10,
                                     Constants.CompoundFrequency.Monthly)));
            val = 100000.00;
            Universal.printf(Convert.ToString(
                val.CompoundInterest(0.018,
                                     30,
                                     Constants.CompoundFrequency.Monthly)));
            Universal.printf(Convert.ToString(
                val.CompoundInterest(0.041,
                                     30,
                                     Constants.CompoundFrequency.Monthly)));
        }
    }
}
