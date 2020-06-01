using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace RADImplementationProject
{
    public class BasicCountSketch
    {
        private Func<ulong, ulong> h = null;
        private Func<ulong, int> s = null;
        private long[] C = null;

        public BasicCountSketch(int t, int q = 89, int seed = -1)
        {
            if (t > 31)
                throw new Exception("t is used in creating C which has to be of max size 2^31, hence t <= 31");

            //generate 4 a's using either a seed or no seed (random for each run)
            BigInteger[] a = new BigInteger[]
            {
                new BigInteger(Generator.GenerateBits(q, seed)),
                new BigInteger(Generator.GenerateBits(q, seed == -1 ? -1 : seed + 1)),
                new BigInteger(Generator.GenerateBits(q, seed == -1 ? -1 : seed + 2)),
                new BigInteger(Generator.GenerateBits(q, seed == -1 ? -1 : seed + 3))
            };

            //Using our Horner's rule implementation, we can get a 4 universal hashfunction by passing 4 a's
            Func<ulong, BigInteger> g = HashFunction.kIndependentMultiplyModPrime(a, q);
            Tuple<Func<ulong, ulong>, Func<ulong, int>> hAnds = HashFunction.CountSketchHashfunctions(g, t, q);
            this.h = hAnds.Item1;
            this.s = hAnds.Item2;
            this.C = new long[1 << t];
        }

        public void Process(Tuple<ulong, long> entry) => this.C[h(entry.Item1)] += this.s(entry.Item1) * entry.Item2;

        public long Query(ulong x) => this.s(x) * this.C[h(x)];

        public BigInteger Estimate2ndMoment() => this.C.Aggregate(BigInteger.Zero, (acc, c) => acc + (long)Math.Pow(c, 2));
    }
}
