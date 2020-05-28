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
    }
}
