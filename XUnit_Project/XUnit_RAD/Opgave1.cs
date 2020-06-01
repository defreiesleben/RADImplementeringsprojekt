using System;
using System.Numerics;
using Xunit;
using RADImplementationProject;
using System.Diagnostics;
using System.Collections.Generic;
using Xunit.Abstractions;

namespace XUnit_RAD
{
    public class Opgave1
    {
        private readonly ITestOutputHelper output;
        public Opgave1(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Test_Multiply_Shift()
        {
            int l = 63; //2^l = 64, hence we get x |-> [0,...,127]
            int seed = 1;
            ulong a = BitConverter.ToUInt64(Generator.MakeOdd(Generator.GenerateBits(64, seed)));
            Func<ulong, ulong> h = HashFunction.MultiplyShift(a, l);
            for (ulong i = 0; i < 100000; i ++) { 
                output.WriteLine(i + ": " + h(i));
                Assert.True(h(i) < (1UL << l));
            }
        }

        [Fact]
        public void Test_Multiply_Mod_Prime()
        {
            int l = 63; //2^l = 64, hence we get x |-> [0,...,127]
            int seed = 1;
            BigInteger a = new BigInteger(Generator.GenerateBits(89, seed));
            BigInteger b = new BigInteger(Generator.GenerateBits(89, seed + 1));
            Func<ulong, ulong> h_actual = HashFunction.MultiplyModPrime(a, b, l);
            Func<ulong, ulong> h_expected = HashFunction.MultiplyModPrimeReal(a, b, l);

            for (ulong x = 3000000; x < 3500000; x++)
            {
                //output.WriteLine("x={0,1}, equal?: {1}\n\t{2}\n\t{3}", x, h_actual(x) == h_expected(x), h_actual(x), h_expected(x));
                Assert.Equal(h_expected(x), h_actual(x));
            }
        }

        [Fact]
        public void Test_Both_Random_Keys_In_Universe()
        {
            int l = 31;
            int seed = 1;
            ulong a_MS = BitConverter.ToUInt64(Generator.MakeOdd(Generator.GenerateBits(64, seed)));
            Func<ulong, ulong> h_MS = HashFunction.MultiplyShift(a_MS, l);
            BigInteger a_MMP = new BigInteger(Generator.GenerateBits(89, seed));
            BigInteger b = new BigInteger(Generator.GenerateBits(89, seed + 1));
            Func<ulong, ulong> h_MMP = HashFunction.MultiplyModPrime(a_MMP, b, l);

            for (ulong x = 0; x < 1000000; x++)
            {
                ulong randomULong = BitConverter.ToUInt64(Generator.GenerateBits(64));
                Assert.True(h_MS(randomULong) < (1UL << l));
                Assert.True(h_MMP(randomULong) < (1UL << l));
            }
        }


        [Fact]
        public void Test_Multiply_Mod_Prime_And_Multiply_Shift_Speed()
        {
            int l = 7; //2^l = 64, hence we get x |-> [0,...,127]
            int seed = 1;

            // Opgave 1 (c)

            BigInteger sum = 0;
            IEnumerable<Tuple<ulong, int>> S = Generator.CreateStream(100000000);
            // Keys generated

            Stopwatch sw = new Stopwatch();

            BigInteger a = new BigInteger(Generator.GenerateBits(89, seed));
            BigInteger b = new BigInteger(Generator.GenerateBits(89, seed + 1));
            Func<ulong, ulong> h_MMP = HashFunction.MultiplyModPrime(a, b, l);

            sw.Restart();
            foreach (Tuple<ulong, int> v in S)
            {
                BigInteger hashedMMP = h_MMP(v.Item1);
                //Console.WriteLine("Key : {0,16} \t HashedMMP Key {1,16} \t Value : {2} "
                //    , v.Item1
                //    , hashedMMP
                //    , v.Item2);
                sum += hashedMMP;
            }
            sw.Stop();
            long msMMP = sw.ElapsedMilliseconds;
            output.WriteLine("Sum of hashedMMP values {0, 15}  Elapsed ms : {1}", sum, msMMP);

            ulong _a = BitConverter.ToUInt64(Generator.MakeOdd(Generator.GenerateBits(64, seed)));
            Func<ulong, ulong> h_MS = HashFunction.MultiplyShift(_a, l);

            sw.Restart();
            sum = 0;
            foreach (Tuple<ulong, int> v in S)
            {
                BigInteger hashedMS = h_MS(v.Item1);
                //Console.WriteLine("Key : {0,16} \t HashedMS Key {1,16} \t Value : {2} "
                //    , v.Item1
                //    , hashedMS
                //    , v.Item2);
                sum += hashedMS;
            }
            sw.Stop();
            long msMS = sw.ElapsedMilliseconds;
            output.WriteLine("Sum of hashedMS    values {0,15}  Elapsed ms : {1}", sum, msMS);

            output.WriteLine("MMP vs. MS ratio : {0}%", Math.Round(((double)msMMP / msMS) * 100, 2));
        }
    }
}
