#pragma warning disable CS0162, CS0219, CS0414, CS1587, CS1591, CS1998, IDE0028, IDE0044, IDE0059

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// 
/// Principles applied:
/// 1. All even number (except 2) are not prime.
/// 2. Numbers squared an even number of times will not generate primes.
/// 3. Numbers squared an odd number of times COULD generate primes.
/// 
/// TODO
/// 1. Convert string to number.
///    a. Convert char to digit.
///    b. Convert number to binary.
/// 2. Calculate square root of number.
/// 3. Divide binary numbers.
/// 4. Mod binary numbers.
/// 5. Square root binary numbers.
/// </summary>

using System;
using System.Collections;
using static Extensions.Constants;
using static Extensions.Core;

namespace Extensions
{
    /// <summary>
    /// ***NEW*** value Type designed to hold excessively large numbers
    /// such as Mersenne Prime numbers, hence the name.
    /// 10 bits (1111111111) = 1024 i.e. holds 3 digits.
    /// Cornellion = 1 with 100,000,000 zeros.
    /// Cornellion requires 333,333,334 bits to represent.
    /// </summary>
    [Serializable]
    public partial class Mersenne64
    {
        static int HighestSetBit(ref BitArray b)
        {
            for (int C = b.Count - 1; C >= 0; C--)
            {
                if (b[C] == true) return C;
            }
            return -1;
        }
        static int Compare(ref BitArray a, ref BitArray b)
        {
            // Compare returns 1 if a>b, 0 if a==b or -1 if a<b
            int high = a.Count - 1;
            for (int C = high; C >= 0; C--)
            {
                if (a[C] != b[C])
                {
                    if (a[C] == true) return 1;
                    return -1;
                }
            }
            return 0;
        }
        static void Subtract(ref BitArray a, ref BitArray b)    //also replace number with the subtraction result(b-a)
        {
            int minc = a.Count < b.Count ? a.Count : b.Count;
            bool borrow = false;
            for (int C = 0; C < minc; C++)
            {
                bool diff = a[C] ^ b[C] ^ borrow;
                borrow = (a[C] && b[C] && borrow) || (!a[C] && (b[C] || borrow));
                a[C] = diff;
            }
            for (int C = minc; borrow && C < a.Count; C++)
            {
                a[C] = borrow = !a[C];
            }
        }
        static void myRemainder(ref BitArray number, BitArray divisor)
        {
            int high_n = HighestSetBit(ref number);
            int high_d = HighestSetBit(ref divisor);


            if (high_d > high_n) return;     // to return BitArray number here
            if (high_d == high_n && Compare(ref number, ref divisor) == -1) return;   // nr < d, keep n as r  // also return number here

            int dshift = high_n - high_d;
            //divisor.LeftShift(dshift);

            for (int C = 0; C <= dshift; C++)
            {
                int cmp = Compare(ref number, ref divisor);
                if (cmp == 0)
                {
                    number.SetAll(false);       // d evenly divides nr, so r is 0
                    return;          // return 0 here
                }

                if (cmp > 0)
                {
                    Subtract(ref number, ref divisor);

                }
                //divisor.RightShift(1);
            }
        }
        static char checkEndsWith(char num)
        {
            if (num == '1' ||
               num == '3' ||
               num == '5' ||
               num == '7' ||
               num == '9')
                return '1';
            else
                return '0';
        }
        static void divideByTwo(ref string num)
        {
            string newNum = "";
            int addi = 0;
            for (int C = 0; C < num.Length; C++)
            {
                int new_dgt = ((num[C] - '0') / 2) + addi;
                newNum += (char)(new_dgt + '0');
                addi = (checkEndsWith(num[C]) - '0') * 5;
            }
            if (newNum[0] == '0')
            {
                newNum = newNum.Substring(1);
            }
            num = newNum;
        }
        static void convertToBinary(ref string number)
        {
            string estack = "";
            for (int C = 0; number.Length != 0; C++)
            {
                estack += checkEndsWith(number[number.Length - 1]);
                divideByTwo(ref number);

            }
            number = estack;
        }
        static BitArray developBitArray(ref string str, int len)
        {
            BitArray num = new BitArray(len);
            for (int C = 0; C < str.Length; C++)
            {
                if (str[C] == '0')
                    num[C] = false;
                else
                    num[C] = true;
            }
            return num;
        }
        static BitArray getRemainder(ref string number, ref string divisor)
        {
            convertToBinary(ref number);
            convertToBinary(ref divisor);

            int len = number.Length > divisor.Length ? number.Length : divisor.Length;
            BitArray num = developBitArray(ref number, len);
            BitArray div = developBitArray(ref divisor, len);

            myRemainder(ref num, div);
            return num;

        }
        static void displayResult(ref BitArray rem)
        {
            for (int C = rem.Count - 1; C >= 0; C--)
            {
                if (rem[C] == false)
                    Console.Write('0');
                else
                    Console.Write('1');
            }
            Console.WriteLine();
        }

        public static string rem()
        {
            //string number = "6342874683274789265664879324632987643287648792981999999999364918237648237642398146219387462398476932846329846287321468288";
            //string divisor = "634287468327478926566487932463298764328764879298199999999936491823764823764239814621938746239847693284632984628732146828";
            //string number =  "299999999999999999999999999";
            //string divisor = "18888888888888888888888888";

            //string number = "1844674407370955162";
            //string divisor = "3";
            //string divisor = "888888888888888888888888888888888888888888888";
            //string divisor = "1844674407370955161";

            bool[] bitarray = new bool[536870912];
            Timer resetBitArray = new Timer();
            resetBitArray.Start();
            for (int C = 0; C < 536870912; C++)
            {
                bitarray[C] = true;
            }
            Console.WriteLine(resetBitArray.Stop());

            string number = "1", divisor = "1";
            for (int C = 0; C < 98; C++)
            {
                number += "0";
            }
            number += "1";
            for (int C = 0; C < 49; C++)
            {
                divisor += "0";
            }
            BitArray rem = getRemainder(ref number, ref divisor);
            Console.WriteLine("Remainder:");
            displayResult(ref rem);
            return "";
        }

        private static int NumberOfBits = 536870912;//333333334;
        private static int DividentBeginBit = 0;
        private static int DividentEndBit = 0;
        private static int DividerBeginBit = 0;
        private static int DividerEndBit = 0;
        private static System.Text.StringBuilder sb = new System.Text.StringBuilder();
        /// <summary>
        /// The bit map that holds the bitwise representation of the Mersenne
        /// number.
        /// </summary>
        public bool[] divident = new bool[NumberOfBits];  //64MB
        //private int numericChunk = 0;

        /// <summary>
        /// Class constructor.  Default is to initialize all bits as 1 for
        /// 333,333,334 bits i.e. represent the first number with 
        /// 100,000,000 digits.
        /// </summary>
        /// <param name="InitialValue">A bit array to use as default value.</param>
        public Mersenne64(bool[] InitialValue = null)
        {
            if (InitialValue != null)
            {
                for (int C = InitialValue.Length - 1; C >= 0; C--)
                {
                    divident[NumberOfBits - 1 - C] = InitialValue[C];
                }
            }
            else
            { 
                //333333334 bits represent the first number larger than
                //100 million digits.
                for (int C = NumberOfBits - 333333334; C < NumberOfBits; C++)
                {
                    divident[C] = true;
                }
            }
        }

        public bool[] Mod(bool[] divider)
        {
            DividentBeginBit = GetStartingBit(ref divident);
            DividerBeginBit = GetStartingBit(ref divider);
            if ((divider.Length - DividerBeginBit) > (divident.Length - DividentBeginBit))
            {
                if (DividentBeginBit > 0)
                {
                    return GetBits(ref divider, DividerBeginBit);
                }
                return divider;
            }
            else  //divider is smaller than divident to it CAN divide.
            {
                bool[] chunk = GetBits(ref divident, DividentBeginBit, divider.Length);
                bool[] newchunk = new bool[1];
                DividentEndBit = DividentBeginBit + divider.Length;
                if (!Equal(ref chunk, ref divider))
                {
                    while (!GreaterOrLess(ref chunk, ref divider))
                    {
                        //get another bit
                        newchunk = new bool[chunk.Length + 1];
                        chunk.CopyTo(newchunk, 0);
                        newchunk[newchunk.Length - 1] = divident[DividentBeginBit + chunk.Length];
                        chunk = new bool[newchunk.Length];
                        newchunk.CopyTo(chunk, 0);
                        DividentEndBit++;
                    }
                    ResetArray(ref newchunk);
                    bool borrow = false;
                    int borrowAt = 0;
                    for (int C = chunk.Length - 1; C >= 0; C--)
                    {
                        //Chunk bit is 0 and Divider bit is 1.
                        if (Convert.ToByte(chunk[C]) < Convert.ToByte(divider[C]))
                        {
                            if (!borrow)
                            {
                                borrow = true;  //Need to borrow from bits to the left.
                                borrowAt = C;  //Save the borrow position.
                            }
                        }
                        else
                        {
                            if (borrow)
                            {
                                chunk[C] = false;  //Borrow the bit.
                                //Now shift it right.
                                for (int v = C + 1; v <= borrowAt; v++)
                                {
                                    chunk[v] = true;
                                }

                                //Since we're in binary, we only need to borrow
                                //if the chunk bit is 0 and the divider bit is
                                //1.  As a result, the subtraction of the
                                //divider bit from the borrowed chunk bit is
                                //always going to be 1.
                                newchunk[borrowAt] = true;
                            }
                            //Chunk bit is 1 and Divider bit is 0.
                            if (chunk[C] && (!divider[C]))
                            {
                                newchunk[C] = true;
                            }
                            else  //Both bits = 0 or 1.
                            {
                                newchunk[C] = false;
                            }
                        }
                    }
                }
                else
                {
                    if (DividentEndBit == divident.Length)
                    {
                        //All bits processed.
                        return new bool[1] { false };
                    }
                    else  //More bits remain.
                    {

                    }
                }
                int remainder = Convert.ToInt32(GetBitsAsString(ref chunk, 0), 2) %
                    Convert.ToInt32(GetBitsAsString(ref divider, 0), 2);
                
                int length = divider.Length - DividerBeginBit;
                for (int C = 0; C < length; C++)
                {

                }
            }
            return null;
        }

        public static void ResetArray(ref bool[] array)
        {
            for (int C = 0; C < array.Length; C++)
            {
                array[C] = false;
            }
        }

        public static bool Equal(ref bool[] val1, ref bool[] val2)
        {
            for (int C = 0; C < val1.Length; C++)
            {
                if (val1[C] != val2[C])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool GreaterOrLess(ref bool[] val1, 
                                         ref bool[] val2, 
                                         MersenneComparisonType type = 
                                             MersenneComparisonType.Greater)
        {
            int difference = 0;
            //Arrays are different lengths, val2 is longer.
            if (val1.Length < val2.Length)
            {
                difference = val2.Length - val1.Length;
                //Examine the N difference in length characters.
                for (int C = 0; C < difference; C++)
                {
                    //If the longer array has a 1 bit, it is greater.
                    if (val2[C])
                    {
                        return type == MersenneComparisonType.Greater ? 
                            false : true; //Since val2 > val1.
                    }
                }
                //Now evaluate bits from least significant bit.
                for (int C = val2.Length; C > 0; C--)
                {
                    if (val1[C - difference - 1] != val2[C - 1])
                    {
                        if (val1[C])
                        {
                            return type == MersenneComparisonType.Greater ?
                                true : false; //Since val1 > val2.
                        }
                        else
                        {
                            return type == MersenneComparisonType.Greater ?
                                false : true; //Since val2 > val1.
                        }
                    }
                }
            }
            //Arrays are different lengths, val1 is longer.
            if (val1.Length > val2.Length)
            {
                difference = val1.Length - val2.Length;
                //Examine the N difference in length characters.
                for (int C = 0; C < difference; C++)
                {
                    //If the longer array has a 1 bit, it is greater.
                    if (val1[C])
                    {
                        return type == MersenneComparisonType.Greater ?
                            true : false; //Since val1 > val2.
                    }
                }
                //Now evaluate bits from least significant bit.
                for (int C = val1.Length; C > 0; C--)
                {
                    if (val2[C - difference - 1] != val1[C - 1])
                    {
                        if (val2[C])
                        {
                            return type == MersenneComparisonType.Greater ?
                                false : true; //Since val2 > val1.
                        }
                        else
                        {
                            return type == MersenneComparisonType.Greater ?
                                true : false; //Since val1 > val2.
                        }
                    }
                }
            }
            //Array lengths are equal.
            if (val1.Length == val2.Length)
            {
                //Now evaluate bits from most significant bit.
                for (int C = 0; C < val1.Length; C++)
                {
                    if (val1[C] != val2[C])
                    {
                        if (val1[C])
                        {
                            return type == MersenneComparisonType.Greater ?
                                true : false; //Since val1 > val2.
                        }
                        else
                        {
                            return type == MersenneComparisonType.Greater ?
                                false : true; //Since val2 > val1.
                        }
                    }
                }
            }
            return false;  // val1 == val2
        }

        public static bool GreaterThan(ref bool[] val1, ref bool[] val2)
        {
            int difference = 0;
            //Arrays are different lengths, val2 is longer.
            if (val1.Length < val2.Length)
            {
                difference = val2.Length - val1.Length;
                //Examine the N difference in length characters.
                for (int C = 0; C < difference; C++)
                {
                    //If the longer array has a 1 bit, it is greater.
                    if (val2[C])
                    {
                        return false;  //Since val2 > val1.
                    }
                }
                //Now evaluate bits from least significant bit.
                for (int C = Bigest(val1.Length, val2.Length); C > 0; C--)
                {
                    if (val1[C - difference - 1] != val2[C - 1])
                    {
                        if (val1[C])
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            //Arrays are different lengths, val1 is longer.
            if (val1.Length > val2.Length)
            {
                difference = val1.Length - val2.Length;
                //Examine the N difference in length characters.
                for (int C = 0; C < difference; C++)
                {
                    //If the longer array has a 1 bit, it is greater.
                    if (val1[C])
                    {
                        return true;  //Since val1 > val2.
                    }
                }
                //Now evaluate bits from least significant bit.
                for (int C = Bigest(val1.Length, val2.Length); C > 0; C--)
                {
                    if (val2[C - difference - 1] != val1[C - 1])
                    {
                        if (val2[C])
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            return false;  // val1 == val2
        }

        public static bool LessThan(ref bool[] val1, ref bool[] val2)
        {
            int difference = 0;
            //Arrays are different lengths, val2 is longer.
            if (val1.Length < val2.Length)
            {
                difference = val2.Length - val1.Length;
                //Examine the N difference in length characters.
                for (int C = 0; C < difference; C++)
                {
                    //If the longer array has a 1 bit, it is greater.
                    if (val2[C])
                    {
                        return true;  //Since val2 > val1.
                    }
                }
                //Now evaluate bits from least significant bit.
                for (int C = Bigest(val1.Length, val2.Length); C > 0; C--)
                {
                    if (val1[C - difference - 1] != val2[C - 1])
                    {
                        if (val1[C])
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            //Arrays are different lengths, val1 is longer.
            if (val1.Length > val2.Length)
            {
                difference = val1.Length - val2.Length;
                //Examine the N difference in length characters.
                for (int C = 0; C < difference; C++)
                {
                    //If the longer array has a 1 bit, it is greater.
                    if (val1[C])
                    {
                        return false;  //Since val1 > val2.
                    }
                }
                //Now evaluate bits from least significant bit.
                for (int C = Bigest(val1.Length, val2.Length); C > 0; C--)
                {
                    if (val2[C - difference - 1] != val1[C - 1])
                    {
                        if (val2[C])
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return false;  // val1 == val2
        }

        public static int GetStartingBit(ref bool[] value)
        {
            for (int C = 0; C < value.Length; C++)
            {
                if (value[C])
                {
                    return C;
                    break;
                }
            }
            return value.Length;
        }

        public static bool[] GetBits(ref bool[] value, int offset, int length = 0)
        {
            int size = value.Length - offset;
            if (value.Length - offset - length > 0)
            {
                size = length;
            }
            bool[] result = new bool[size];
            int counter = 0;
            for (int C = offset; C < (offset + size); C++)
            {
                result[counter] = value[C];
                counter++;
            }
            return result;
        }

        public static string GetBitsAsString(ref bool[] value, int offset)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            for (int C = offset; C < (value.Length); C++)
            {
                if (value[C])
                {
                    result.Append("1");
                }
                else
                {
                    result.Append("0");
                }
            }
            return result.ToString();
        }

        //public Mersenne40(ref string numberAsString)
        //{
        //    byte counter = 0;
        //    while (numberAsString.Length > 0)
        //    {
        //        if (numberAsString.Length > 3)
        //        {
        //            numericChunk = System.Int32.Parse(numberAsString.Substring(3, SubstringType.FromTail));
        //            numberAsString = numberAsString.Substring(0, numberAsString.Length - 3);
        //            numericChunk += 23;
        //            while (numericChunk > 0)
        //            {
        //                bitmap[DividentBeginBit] = (numericChunk % 2 != 0);
        //                DividentBeginBit--;
        //                counter++;
        //                numericChunk = numericChunk / 2;
        //            }
        //            if (counter != 10)
        //            {
        //                DividentBeginBit -= (10 - counter);
        //                counter = 0;
        //            }
        //            else
        //            {
        //                numericChunk = System.Int32.Parse(numberAsString.Substring(3, SubstringType.FromTail));
        //            }
        //        }
        //        else
        //        {
        //            numericChunk = System.Int32.Parse(numberAsString);
        //            numberAsString = "";
        //            while (numericChunk > 0)
        //            {
        //                bitmap[DividentBeginBit] = (numericChunk % 2 != 0);
        //                DividentBeginBit--;
        //                counter++;
        //                numericChunk = numericChunk / 2;
        //            }
        //            if (counter != 10)
        //            {
        //                DividentBeginBit -= (10 - counter);
        //                counter = 0;
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="divident"></param>
        /// <param name="dividor"></param>
        /// <returns></returns>
        public static Mersenne64 operator /(Mersenne64 divident, Mersenne64 dividor)
        {
            int dividorOffset = 0;
            for (int C = 0; C < NumberOfBits; C++)
            {
                if (dividor.divident[C])
                {
                    dividorOffset = C;
                    break;
                }
            }
            for (int C = 0; C < NumberOfBits; C++)
            {
                if (divident.divident[C])
                {
                    //if (LessThan(divident.bitmap[C], dividor.bitmap[C]))
                }
            }
            return null;
        }

        public class BaseConverter
        {
            public static string ToBinary(int n)
            {
                if (n < 2) return n.ToString();
                var divisor = n / 2;
                var remainder = n % 2;
                return ToBinary(divisor) + remainder;
            }

            public static System.Numerics.BigInteger Sqrt(System.Numerics.BigInteger n)
            {
                if (n == 0) return 0;
                if (n > 0)
                {
                    int bitLength = System.Convert.ToInt32(Math.Ceiling(System.Numerics.BigInteger.Log(n, 2)));
                    System.Numerics.BigInteger root = System.Numerics.BigInteger.One << (bitLength / 2);

                    while (!isSqrt(n, root))
                    {
                        root += n / root;  //FIX THIS
                        root /= 2;
                        Console.WriteLine($"Number:{n.ToString().Length} digits / Root:{root.ToString().Length} digits");
                    }

                    return root;
                }

                throw new ArithmeticException("NaN");
            }

            private static Boolean isSqrt(System.Numerics.BigInteger n, System.Numerics.BigInteger root)
            {
                System.Numerics.BigInteger lowerBound = root * root;
                System.Numerics.BigInteger upperBound = (root + 1) * (root + 1);

                return (n >= lowerBound && n < upperBound);
            }

            //public static int Sqrt(int num)
            //{
            //    //assert(("sqrt input should be non-negative", num > 0));
            //    int res = 0;
            //    int bit = 1 << 30; // The second-to-top bit is set.
            //                           // Same as ((unsigned) INT32_MAX + 1) / 2.

            //    // "bit" starts at the highest power of four <= the argument.
            //    while (bit > num)
            //        bit >>= 2;

            //    while (bit != 0)
            //    {
            //        if (num >= res + bit)
            //        {
            //            num -= res + bit;
            //            res = (res >> 1) + bit;
            //        }
            //        else
            //            res >>= 1;
            //        bit >>= 2;
            //    }
            //    return res;
            //}

            //Convert number in string representation from base:from to base:to. 
            //Return result as a string
            public static String Convert(int from, int to, String decimalNumber)
            {
                //Return error if input is empty
                if (String.IsNullOrEmpty(decimalNumber))
                {
                    return ("Error: Nothing in Input String");
                }
                //only allow uppercase input characters in string
                decimalNumber = decimalNumber.ToUpper();

                //only do base 2 to base 36 (digit represented by characters 0-Z)"
                if (from < 2 || from > 36 || to < 2 || to > 36)
                { 
                    return ("Base requested outside range"); 
                }

                //convert string to an array of integer digits representing number in base:from
                int decimalLength = decimalNumber.Length;
                int[] decimalArray = new int[decimalLength];
                int k = 0;
                for (int C = decimalNumber.Length - 1; C >= 0; C--)
                {
                    if (decimalNumber[C] >= '0' && decimalNumber[C] <= '9') 
                    {
                        //fs[k++] = (int)(s[i] - '0'); 
                        decimalArray[C] = (int)(decimalNumber[C] - '0');
                    }
                    else
                    {
                        if (decimalNumber[C] >= 'A' && decimalNumber[C] <= 'Z') 
                        {
                            //fs[k++] = 10 + (int)(s[i] - 'A'); 
                            decimalArray[C] = 10 + (int)(decimalNumber[C] - 'A');
                        }
                        else
                        {
                            return ("Error: Input string must only contain any of 0 - 9 or A-Z"); 
                        } //only allow 0-9 A-Z characters
                    }
                }

                //check the input for digits that exceed the allowable for base:from
                foreach (int C in decimalArray)
                {
                    if (C >= from) 
                    { 
                        return ("Error: Not a valid number for this input base"); 
                    }
                }

                //find how many digits the output needs
                int binaryLength = decimalLength * (from / to + 1);
                int[] binaryArray = new int[binaryLength + 10]; //assign accumulation array
                int[] resultArray = new int[binaryLength + 10]; //assign the result array
                binaryArray[0] = 1; //initialize array with number 1 

                //evaluate the output
                for (int decimalArrayIndex = 0; decimalArrayIndex < decimalLength; decimalArrayIndex++) //for each input digit
                {
                    for (int binaryArrayIndex = 0; binaryArrayIndex < binaryLength; binaryArrayIndex++) //add the input digit 
                                                    // times (base:to from^i) to the output cumulator
                    {
                        resultArray[binaryArrayIndex] += binaryArray[binaryArrayIndex] * decimalArray[decimalArrayIndex];
                        int temp = resultArray[binaryArrayIndex];
                        int rem = 0;
                        int ip = binaryArrayIndex;
                        do // fix up any remainders in base:to
                        {
                            rem = temp / to;
                            resultArray[ip] = temp - rem * to; 
                            ip++;
                            resultArray[ip] += rem;
                            temp = resultArray[ip];
                        }
                        while (temp >= to);
                    }

                    //calculate the next power from^i) in base:to format
                    for (int binaryArrayIndex = 0; binaryArrayIndex < binaryLength; binaryArrayIndex++)
                    {
                        binaryArray[binaryArrayIndex] = binaryArray[binaryArrayIndex] * from;
                    }
                    for (int binaryArrayIndex = 0; binaryArrayIndex < binaryLength; binaryArrayIndex++) //check for any remainders
                    {
                        int temp = binaryArray[binaryArrayIndex];
                        int rem = 0;
                        int ip = binaryArrayIndex;
                        do  //fix up any remainders
                        {
                            rem = temp / to;
                            binaryArray[ip] = temp - rem * to; ip++;
                            binaryArray[ip] += rem;
                            temp = binaryArray[ip];
                        }
                        while (temp >= to);
                    }
                }

                //convert the output to string format (digits 0,to-1 converted to 0-Z characters) 
                String sout = String.Empty; //initialize output string
                bool first = false; //leading zero flag
                for (int C = binaryLength; C >= 0; C--)
                {
                    if (resultArray[C] != 0) 
                    { 
                        first = true; 
                    }
                    if (!first) 
                    { 
                        continue; 
                    }
                    if (resultArray[C] < 10) 
                    { 
                        sout += (char)(resultArray[C] + '0'); 
                    }
                    else 
                    { 
                        sout += (char)(resultArray[C] + 'A' - 10); 
                    }
                }
                if (String.IsNullOrEmpty(sout)) 
                { 
                    return "0"; 
                } //input was zero, return 0
                                                                //return the converted string
                return sout;
            }
        }
    }
}
#pragma warning restore CS0162, CS0219, CS0414, CS1587, CS1591, CS1998, IDE0028, IDE0044, IDE0059
