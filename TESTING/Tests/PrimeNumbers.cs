//#pragma warning disable CS1587

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Extensions.Core;

namespace TESTING
{
    public static class PrimeNumbers
    {
        public static void Test()
        {
            printf("********* Prime Numbers Testing *********", ConsoleColor.Green);
            GenerateNSamples(1);
        }

        #region GenerateNSamples
        public static void GenerateNSamples(System.UInt64 itterations)
        {
            TimeSpan ts;
            System.DateTime start;
            List<TimeSpan> sync = new List<TimeSpan>();
            List<TimeSpan> async = new List<TimeSpan>();
            System.UInt64 multiplier = 10000000;
            for (System.UInt64 C = 0; C < itterations; C++)
            {
                printf($"Calculating the {multiplier / 1000000} millionth prime asynchronously", ConsoleColor.Yellow);
                start = System.DateTime.Now;
                printf(start);
                //printf(GetNthPrimeAsync(multiplier).Result);
                ts = System.DateTime.Now - start;
                printf(ts.TotalSeconds);
                async.Add(ts);
                printf($"Calculating the {multiplier / 1000000} millionth prime synchronously", ConsoleColor.Yellow);
                start = System.DateTime.Now;
                printf(start);
                //printf(GetNthPrime(multiplier));
                ts = System.DateTime.Now - start;
                sync.Add(ts);
                printf(ts.TotalSeconds);
                C++;
                printf($"============={C}===================");
            }
            printf($"Itterations: [{itterations}]");
            System.UInt64 primes = itterations * multiplier;
            printf($"Primes calculated: [{primes / 1000000}million]");
            double syncTime = sync.Sum(Constants.TimeSpanSumType.Seconds);
            printf($"Synchronous time:  [{syncTime} seconds]");
            double asyncTime = async.Sum(Constants.TimeSpanSumType.Seconds);
            printf($"Asynchronous time: [{asyncTime} seconds]");
            double syncAve = syncTime / primes;
            printf($"Sync ave/prime:    [{syncAve}]");
            double asyncAve = asyncTime / primes;
            printf($"Async ave/prime:   [{asyncAve}]");
            printf($"Async faster than Sync by: [{syncTime / asyncTime}]");
            printf($"% Speed Improvement: [{((syncTime / asyncTime) * 100)}%]");
        }
        #endregion GenerateNSamples
    }
}
//#pragma warning restore CS1587
