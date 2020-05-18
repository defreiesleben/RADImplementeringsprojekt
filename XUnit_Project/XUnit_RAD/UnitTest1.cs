using System;
using System.Numerics;
using Xunit;
using RADImplementationProject;
using System.Diagnostics;
using System.Collections.Generic;

namespace XUnit_RAD
{
    public class UnitTest1
    {
        [Fact]
        public void Test_Multiply_Mod_Prime()
        {
            int l = 63; //2^l = 64, hence we get x |-> [0,...,127]

            for (ulong x = 3000000; x < 3500000; x++)
            {
                BigInteger actual = HashFunctions.MultiplyModPrime(x, l);
                BigInteger expected = HashFunctions.MultiplyModPrimeReal(x, l);
                //Console.WriteLine("x={0,1}, equal?: {1}\n\t{2}\n\t{3}", x, actual == expected, actual, expected);
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void Test_Multiply_Mod_Prime_Speed()
        {
            int l = 7; //2^l = 64, hence we get x |-> [0,...,127]

            // Opgave 1 (c)

            BigInteger sum = 0;
            IEnumerable<Tuple<ulong, int>> S = Generator.CreateStream(1000000);
            // Keys generated

            Stopwatch sw = new Stopwatch();

            sw.Restart();
            foreach (Tuple<ulong, int> v in S)
            {
                BigInteger hashedMMP = HashFunctions.MultiplyModPrime(v.Item1, l);
                //Console.WriteLine("Key : {0,16} \t HashedMMP Key {1,16} \t Value : {2} "
                //    , v.Item1
                //    , hashedMMP
                //    , v.Item2);
                sum += hashedMMP;
            }
            sw.Stop();
            long msMMP = sw.ElapsedMilliseconds;
            Console.WriteLine("Sum of hashedMMP values {0} \t Elapsed ms : {1}", sum, msMMP);

            sw.Restart();
            sum = 0;
            foreach (Tuple<ulong, int> v in S)
            {
                BigInteger hashedMS = HashFunctions.MultiplyShift(v.Item1, l);
                //Console.WriteLine("Key : {0,16} \t HashedMS Key {1,16} \t Value : {2} "
                //    , v.Item1
                //    , hashedMS
                //    , v.Item2);
                sum += hashedMS;
            }
            sw.Stop();
            long msMS = sw.ElapsedMilliseconds;
            Console.WriteLine("Sum of hashedMS values {0} \t Elapsed ms : {1}", sum, msMS);

            Console.WriteLine("MMP vs. MS ratio : {0}", ((double)msMMP / msMS) * 100);
        }
    }
}
