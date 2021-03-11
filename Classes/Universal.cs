#pragma warning disable CS1587, CS1998, IDE0059, IDE0028

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using System.Collections.Generic;

namespace Extensions
{
    /// <summary>
    /// Universal class for constants and enums.
    /// </summary>
    public static class Universal
    {
        /// <summary>
        /// The assembly used on out calls.
        /// </summary>
        private static System.Reflection.Assembly assembly;
        /// <summary>
        /// The private object used to manage locks on file I/O.
        /// </summary>
        private static readonly object lockManager = new object();
        /// <summary>
        /// String path where to .Save() and .Load() the prime number list
        /// so as to avoid recalculating the same numbers repeatedly.
        /// </summary>
        public static string PrimeStatePath = "primes.state";

        #region Bigest()
        /// <summary>
        /// Returns the smallest of two given values.
        /// </summary>
        /// <param name="val1">The first value to compare.</param>
        /// <param name="val2">The second value to compare.</param>
        /// <returns>The smallest of the two given values.</returns>
        public static int Bigest(int val1, int val2)
        {
            if (val1 > val2) return val1;
            if (val2 > val1) return val2;
            return val1;  //val1 == val2
        }
        #endregion Bigest()

        #region GetExecutingAssembly...()
        /// <summary>
        /// Gets the current assembly through reflection.
        /// </summary>
        /// <returns>The current Entry or Executing assembly.</returns>
        public static System.Reflection.Assembly GetExecutingAssembly()
        {
            return System.Reflection.Assembly.GetEntryAssembly() == null ?
                System.Reflection.Assembly.GetExecutingAssembly() :
                System.Reflection.Assembly.GetEntryAssembly();
        }

        /// <summary>
        /// Gets the name of the current assembly.
        /// </summary>
        /// <param name="asm">Out parm to hold the assembly.</param>
        /// <param name="escaped">Should the value be escaped?</param>
        /// <returns>Returns the name of the current assembly, optionally 
        /// escaped.</returns>
        private static string GetExecutingAssemblyName(
            out System.Reflection.Assembly asm,
            bool escaped = false)
        {
            asm = GetExecutingAssembly();
            string result = asm.ManifestModule.Name;
            if (escaped)
            {
                return System.Uri.EscapeDataString(result);
            }
            return result;
        }

        /// <summary>
        /// Gets the name of the current assembly.
        /// </summary>
        /// <param name="escaped">Should the value be escaped?</param>
        /// <returns>Returns the name of the current assembly, optionally 
        /// escaped.</returns>
        public static string GetExecutingAssemblyName(bool escaped = false)
        {
            return GetExecutingAssemblyName(out assembly, escaped);
        }

        /// <summary>
        /// Gets the folder path of the current assembly.
        /// </summary>
        /// <param name="asm">Out parm to hold the assembly.</param>
        /// <param name="escaped">Should the value be escaped?</param>
        /// <returns>Returns the folder path of the current assembly, 
        /// optionally escaped.</returns>
        private static string GetExecutingAssemblyFolder(
            out System.Reflection.Assembly asm,
            bool escaped = false)
        {
            asm = GetExecutingAssembly();
            string result = System.IO.Path.GetDirectoryName(asm.Location)
                .TrimEnd('\\');
            if (escaped)
            {
                return System.Uri.EscapeDataString(result);
            }
            return result;
        }

        /// <summary>
        /// Gets the folder path of the current assembly.
        /// </summary>
        /// <param name="asm">Out parm to hold the assembly.</param>
        /// <param name="escaped">Should the value be escaped?</param>
        /// <returns>Returns the folder path of the current assembly, 
        /// optionally escaped.</returns>
        public static string GetExecutingAssemblyFolder(bool escaped = false)
        {
            return GetExecutingAssemblyFolder(out assembly, escaped);
        }

        /// <summary>
        /// Gets the full path and file name of the current assembly.
        /// </summary>
        /// <param name="escaped">Should the value be escaped?</param>
        /// <returns>Returns the full path and file name of the current
        /// assembly, optionally escaped.</returns>
        public static string GetExecutingAssemblyFullPath(bool escaped = false)
        {
            string result = GetExecutingAssemblyFolder(out assembly) + "\\" +
                assembly.ManifestModule.Name;
            if (escaped)
            {
                return System.Uri.EscapeDataString(result);
            }
            return result;
        }
        #endregion GetExecutingAssembly...()

        #region GetFQDN()
        /// <summary>
        /// Returns the current computer Fully Qualified Domain Name.
        /// </summary>
        /// <returns>Returns the current computer Fully Qualified Domain Name.</returns>
        public static string GetFQDN()
        {
            string domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string hostName = System.Net.Dns.GetHostName();
            if (domainName == "")
            {
                return hostName;
            }
            domainName = "." + domainName;
            if (!hostName.EndsWith(domainName))
            {
                hostName = domainName;
            }
            return hostName;
        }
        #endregion GetFQDN()

        #region IsEven()
        /// <summary>
        /// Checks if the given number is even.
        /// </summary>
        /// <param name="number">The given number to check.</param>
        /// <returns>True if the number is even, else False.</returns>
        private static bool IsEven(this System.Numerics.BigInteger number)
        {
            Validate(Constants.ErrorTypeAll, number);
            return (number % 2 == 0);
        }
        #endregion IsEven()

        #region IsOdd()
        /// <summary>
        /// Checks if the given number is odd.
        /// </summary>
        /// <param name="number">The given number to check.</param>
        /// <returns>True if the number is odd, else False.</returns>
        private static bool IsOdd(this System.Numerics.BigInteger number)
        {
            Validate(Constants.ErrorTypeAll, number);
            return (number % 2 != 0);
        }
        #endregion IsOdd()

        #region PrimeNumbers
        /// <summary>
        /// Checks if the given number is a prime number.
        /// </summary>
        /// <param name="number">The given number to check.</param>
        /// <returns>True if the given number is a prime number, else False.</returns>
        private static bool IsPrime(this System.Numerics.BigInteger number)
        {
            Validate(Constants.ErrorTypeAll, number);
            if (number == 2) return true;
            if (IsEven(number)) return false;
            bool prime = true;
            for (long C = 3; C <= Sqrt(number); C += 2) //(Math.Ceiling(Math.Sqrt(number))); C+=2)
            {
                if ((number % C) == 0)
                {
                    prime = false;
                    break;
                }
            }
            return prime;
        }

        private static System.Numerics.BigInteger Sqrt(
            this System.Numerics.BigInteger number)
        {
            System.Numerics.BigInteger n = 0, p = 0;
            if (number == System.Numerics.BigInteger.Zero)
                return System.Numerics.BigInteger.Zero;
            var high = number >> 1;
            var low = System.Numerics.BigInteger.Zero;
            while (high > low + 1)
            {
                n = (high + low) >> 1;
                p = n * n;
                if (number < p)
                    high = n;
                else if (number > p)
                    low = n;
                else
                    break;
            }
            return number == p ? n : low;
        }

        /// <summary>
        /// Gets the requested Nth prime number.
        /// </summary>
        /// <param name="Nth">The count number of the requested prime.</param>
        /// <returns>The requested Nth prime number.</returns>
        public static long GetNthPrime(long Nth)
        {
            List<long> lst = new List<long>();
            lst = lst.Load(PrimeStatePath);
            long current = 3;
            if (lst == null)
            {
                lst = new List<long>();
                lst.Add(2);
                lst.Add(3);
            }
            if (lst.Count >= Nth)
            {
                return lst[(int)Nth - 1];
            }
            else
            {
                current = lst[lst.Count - 1];
            }
            while (lst.Count < Nth)
            {
                current += 2;
                if (ULongExtensions.IsPrime((ulong)current))
                {
                    lst.Add(current);
                }
            }
            lst.Save(PrimeStatePath);
            return lst[(int)Nth - 1];
        }

        /// <summary>
        /// Gets the requested Nth prime number using asynchronous, parallel
        /// processing techniques.
        /// </summary>
        /// <param name="Nth">The count number of the requested prime.</param>
        /// <returns>The requested Nth prime number.</returns>
        public static async System.Threading.Tasks.Task<long> GetNthPrimeAsync(long Nth)
        {
            List<long> lst = new List<long>();
            lst = lst.Load(PrimeStatePath);
            long current = 3;
            if (lst == null)
            {
                lst = new List<long>();
                lst.Add(2);
                lst.Add(3);
            }
            if (lst.Count >= Nth)
            {
                return lst[(int)Nth - 1];
            }
            else
            {
                current = lst[lst.Count - 1];
            }
            //List<System.Threading.Tasks.Task> tasks = new List<System.Threading.Tasks.Task>();
            int cpus = Environment.ProcessorCount - 1;
            while (lst.Count < Nth)
            {
                //for (int C = 0; C < cpus; C++)
                //{
                //    current += 2;
                //    tasks.Add(IsPrimeAsync(new PrimeNumber(current)));
                //}
                current += 2;
                var task1 = IsPrimeAsync(new PrimeNumber(current));
                current += 2;
                var task2 = IsPrimeAsync(new PrimeNumber(current));
                current += 2;
                var task3 = IsPrimeAsync(new PrimeNumber(current));
                current += 2;
                var task4 = IsPrimeAsync(new PrimeNumber(current));
                current += 2;
                var task5 = IsPrimeAsync(new PrimeNumber(current));
                current += 2;
                var task6 = IsPrimeAsync(new PrimeNumber(current));
                current += 2;
                var task7 = IsPrimeAsync(new PrimeNumber(current));
                PrimeNumber[] primes = await System.Threading.Tasks.Task.WhenAll(task1, task2, task3, task4, task5, task6, task7);
                if (task1.Result.IsPrime)
                {
                    lst.Add(task1.Result.Number);
                }
                if (task2.Result.IsPrime)
                {
                    lst.Add(task2.Result.Number);
                }
                if (task3.Result.IsPrime)
                {
                    lst.Add(task3.Result.Number);
                }
                if (task4.Result.IsPrime)
                {
                    lst.Add(task4.Result.Number);
                }
                if (task5.Result.IsPrime)
                {
                    lst.Add(task5.Result.Number);
                }
                if (task6.Result.IsPrime)
                {
                    lst.Add(task6.Result.Number);
                }
                if (task7.Result.IsPrime)
                {
                    lst.Add(task7.Result.Number);
                }
                //if (current.IsPrimeAsync(true))
                //{
                //    lst.Add(current);
                //}
            }
            lst.Save(PrimeStatePath);
            return lst[(int)Nth - 1];
        }

        /// <summary>
        /// Internal async method to determin
        /// </summary>
        /// <param name="prime"></param>
        /// <returns></returns>
        private async static System.Threading.Tasks.Task<PrimeNumber> IsPrimeAsync(PrimeNumber prime)
        {
            for (long C = 3; C < (Math.Ceiling(Math.Sqrt(prime.Number))); C += 2)
            {
                if ((prime.Number % C) == 0)
                {
                    break;
                }
            }
            prime.IsPrime = true;
            return prime;
        }
        #endregion PrimeNumbers

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
                return default;
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

        #region Smallest()
        /// <summary>
        /// Returns the smallest of two given values.
        /// </summary>
        /// <param name="val1">The first value to compare.</param>
        /// <param name="val2">The second value to compare.</param>
        /// <returns>The smallest of the two given values.</returns>
        public static int Smallest(int val1, int val2)
        {
            if (val1 < val2) return val1;
            if (val2 < val1) return val2;
            return val1;  //val1 == val2
        }
        #endregion Smallest()

        #region TimeStamp()
        /// <summary>
        /// Returns the current local date time stamp as a string in either
        /// "YYYY-MM-DD" or "YYYY-MM-DD@hh.mm.ss.nnn" format.
        /// </summary>
        /// <param name="DateOnly">If true, return the current local date
        /// time stamp as "YYYY-MM-DD" else return it as 
        /// "YYYY-MM-DD@hh.mm.ss.nnn"</param>
        /// <returns>Returns the current local date time stamp as a string in
        /// either "YYYY-MM-DD" or "YYYY-MM-DD@hh.mm.ss.nnn" format.</returns>
        public static string TimeStamp(bool DateOnly = false)
        {
            System.DateTime now = System.DateTime.Now;
            if (DateOnly)
            {
                return now.Year.ToString() + "-" +
                    now.Month.ToString("d2") + "-" +
                    now.Day.ToString("d2");
            }
            else
            {
                return now.Year.ToString() + "-" +
                    now.Month.ToString("d2") + "-" +
                    now.Day.ToString("d2") + "@" +
                    now.Hour.ToString("d2") + "." +
                    now.Minute.ToString("d2") + "." +
                    now.Second.ToString("d2") + "." +
                    now.Millisecond.ToString("d3");
            }
        }
        #endregion TimeStamp

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
        public static bool ValidateNoNulls(params object[] parms)
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
        public static bool Validate(Constants.ErrorType[] errors,
                                    params object[] parms)
        {
            for (int C = 0; C < errors.Length; C++)
            {
                switch (errors[C])
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
            return true;
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
        #endregion Validate()
    }
}
#pragma warning restore CS1587, CS1998, IDE0059, IDE0028
