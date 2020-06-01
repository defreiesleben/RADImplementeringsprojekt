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
    public class Opgave4_5
    {
        private readonly ITestOutputHelper output;
        public Opgave4_5(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Test4UniversalMapping()
        {
            int seed = 1;

            BigInteger[] a = new BigInteger[]
            {
                new BigInteger(Generator.GenerateBits(89, seed)),
                new BigInteger(Generator.GenerateBits(89, seed + 1)),
                new BigInteger(Generator.GenerateBits(89, seed + 2)),
                new BigInteger(Generator.GenerateBits(89, seed + 3))
            };

            Func<ulong, BigInteger> g = HashFunction.kIndependentMultiplyModPrime(a);
            for (int t = 0; t <= 64; t++) 
            {
                Tuple<Func<ulong, ulong>, Func<ulong, int>> hAnds = HashFunction.CountSketchHashfunctions(g, t);
                Func<ulong, ulong> h = hAnds.Item1;
                Func<ulong, int> s = hAnds.Item2;
                for (ulong i = 0; i < 100000; i++)
                {
                    Assert.True(h(i) < (1UL << t));
                    Assert.True(Math.Abs(s(i)) == 1);
                }
            }
        }

        [Fact]
        public void TestEstimationOfS_Comparison()
        {
            int n = 100000;

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
                HashTableChaining<long> tableMMP = new HashTableChaining<long>(HashFunction.MultiplyModPrime(a, b, l), 1UL << l);
                sw.Restart();
                foreach (Tuple<ulong, long> elem in S)
                    tableMMP.increment(elem.Item1, (NumberLong)elem.Item2);
                sw.Stop();
                long tmp1 = sw.ElapsedMilliseconds;
                sw.Restart();
                BigInteger Real_S_MMP = Generator.RealCount<long>(tableMMP);
                sw.Stop();
                long tmp2 = sw.ElapsedMilliseconds;
                //output.WriteLine("\tMultiplyShift:");
                //output.WriteLine("\t\tTime to increment:  " + tmp1);
                //output.WriteLine("\t\tTime for summation: " + tmp2);
                //output.WriteLine("\t\tSum: " + Real_S_MS);

                BigInteger[] a_s = new BigInteger[]
                {
                    new BigInteger(Generator.GenerateBits(89, 1)),
                    new BigInteger(Generator.GenerateBits(89, 2)),
                    new BigInteger(Generator.GenerateBits(89, 3)),
                    new BigInteger(Generator.GenerateBits(89, 4))
                };
                HashTableChaining<long> table4MMP = new HashTableChaining<long>(HashFunction.CountSketchHashfunctions(HashFunction.kIndependentMultiplyModPrime(a_s), l).Item1, 1UL << l);
                sw.Restart();
                foreach (Tuple<ulong, long> elem in S)
                    table4MMP.increment(elem.Item1, (NumberLong)elem.Item2);
                sw.Stop();
                long tmp3 = sw.ElapsedMilliseconds;
                sw.Restart();
                BigInteger Real_S_4MMP = Generator.RealCount<long>(table4MMP);
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

                output.WriteLine(l + " & " + n + " & " + tmp1 + " & " + tmp3 + " & " + diff1 + "x & " + tmp2 + " & " + tmp4 + " & " + diff2 + @"x \\\hline");

            }
        }
    }
}
