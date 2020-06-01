using RADImplementationProject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace XUnit_RAD
{
    public class Opgave3
    {
        private readonly ITestOutputHelper output;
        public Opgave3(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void TestEstimationOfS_Both()
        {
            int n = 10000000;

            int l_min = 0; // Should be single hashtable node
            int l_max = 20; // array boundary, as we shift 1 << 30 <=> 2^31 https://docs.microsoft.com/en-us/dotnet/api/system.int32.maxvalue?view=netcore-3.1
            int l = l_min;
            int seed = 2;

            ulong a_64odd = BitConverter.ToUInt64(Generator.MakeOdd(Generator.GenerateBits(64, seed)));
            BigInteger a = new BigInteger(Generator.GenerateBits(89, seed));
            BigInteger b = new BigInteger(Generator.GenerateBits(89, seed + 1));

            Stopwatch sw = new Stopwatch();
            while (l <= l_max)
            {
                sw.Restart();
                HashTableChaining<long> tableMS = new HashTableChaining<long>(HashFunction.MultiplyShift(a_64odd, l), 1UL << l);
                HashTableChaining<long> tableMMP = new HashTableChaining<long>(HashFunction.MultiplyModPrime(a, b, l), 1UL << l);
                IEnumerable<Tuple<ulong, long>> S = Generator.CreateStreamLong(n, l, seed);

                foreach (Tuple<ulong, long> elem in S)
                {
                    tableMS.increment(elem.Item1, (NumberLong)elem.Item2);
                    tableMMP.increment(elem.Item1, (NumberLong)elem.Item2);
                }

                BigInteger Real_S_MS = Generator.RealCount<long>(tableMS);
                BigInteger Real_S_MMP = Generator.RealCount<long>(tableMMP);
                sw.Stop();

                //output.WriteLine("MS table: \n " + tableMS);
                //output.WriteLine("MMP table: \n " + tableMMP);
                output.WriteLine("Iteration: {0,2}, time in ms: {1}", l, sw.ElapsedMilliseconds);
                output.WriteLine("\tMS table sum: " + Real_S_MS);
                output.WriteLine("\tMMP table sum: " + Real_S_MMP);
                l++;
            }
        }


        [Fact]
        public void TestEstimationOfS_Comparison()
        {
            int n = 10000000;

            int l_min = 1;
            int l_max = 29; // array boundary, as we shift 1 << 31 <=> 2^31 https://docs.microsoft.com/en-us/dotnet/api/system.int32.maxvalue?view=netcore-3.1
            int seed = 2;

            ulong a_64odd = BitConverter.ToUInt64(Generator.MakeOdd(Generator.GenerateBits(64, seed)));
            BigInteger a = new BigInteger(Generator.GenerateBits(89, seed));
            BigInteger b = new BigInteger(Generator.GenerateBits(89, seed + 1));

            //same S for all tests

            IEnumerable<Tuple<ulong, long>> S = Generator.CreateStreamLong(n, l_max, seed);
            Stopwatch sw = new Stopwatch();
            for (int l = l_min; l <= l_max; l++) 
            {
                if (l == 17)
                    n /= 10;
                //output.WriteLine("Iteration: {0,2}", l);
                HashTableChaining<long> tableMS = new HashTableChaining<long>(HashFunction.MultiplyShift(a_64odd, l), 1UL << l);
                sw.Restart();
                foreach (Tuple<ulong, long> elem in S)
                    tableMS.increment(elem.Item1, (NumberLong)elem.Item2);
                sw.Stop();
                long tmp1 = sw.ElapsedMilliseconds;
                sw.Restart();
                BigInteger Real_S_MS = Generator.RealCount<long>(tableMS);
                sw.Stop();
                long tmp2 = sw.ElapsedMilliseconds;
                //output.WriteLine("\tMultiplyShift:");
                //output.WriteLine("\t\tTime to increment:  " + tmp1);
                //output.WriteLine("\t\tTime for summation: " + tmp2);
                //output.WriteLine("\t\tSum: " + Real_S_MS);



                HashTableChaining<long> tableMMP = new HashTableChaining<long>(HashFunction.MultiplyModPrime(a, b, l), 1UL << l);
                sw.Restart();
                foreach (Tuple<ulong, long> elem in S)
                    tableMMP.increment(elem.Item1, (NumberLong)elem.Item2);
                sw.Stop();
                long tmp3 = sw.ElapsedMilliseconds;
                sw.Restart();
                BigInteger Real_S_MMP = Generator.RealCount<long>(tableMMP);
                sw.Stop();
                long tmp4 = sw.ElapsedMilliseconds;
                //output.WriteLine("\tMultiplyModPrime:");
                //output.WriteLine("\t\tTime to increment:  " + tmp3);
                //output.WriteLine("\t\tTime for summation: " + tmp4);
                //output.WriteLine("\t\tSum: " + Real_S_MMP);
                //
                //output.WriteLine("\tComparison:");
                //output.WriteLine("\t\tMS vs. MMP increment:  " + Math.Round(((double)tmp1) / tmp3, 2) + "x");
                //output.WriteLine("\t\tMS vs. MMP summation: " + Math.Round(((double)tmp2) / tmp4, 2) + "x");
                //output.WriteLine("\t\tSame sum: " + (Real_S_MS == Real_S_MMP));

                double diff1 = tmp3 == 0 ? 1.0 : Math.Round(((double)tmp1) / tmp3, 2);
                double diff2 = tmp4 == 0 ? 1.0 : Math.Round(((double)tmp2) / tmp4, 2);
                
                output.WriteLine(l + " & "+ n + " & " + tmp1 + " & " + tmp3 + " & " + diff1 + "x & " + tmp2 + " & " + tmp4 + " & " + diff2 + @"x \\\hline");

            }
        }
    }
}
