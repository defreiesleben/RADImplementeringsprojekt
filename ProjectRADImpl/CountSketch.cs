using System;
using System.Collections.Generic;
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

        public BasicCountSketch(int t, int seed = -1)
        {
            if (t > 31)
                throw new Exception("t is used in creating C which has to be of max size 2^31, hence t <= 31");

            BigInteger[] a = new BigInteger[]
            {
                new BigInteger(Generator.GenerateBits(89, seed)),
                new BigInteger(Generator.GenerateBits(89, seed == -1 ? -1 : seed + 1)),
                new BigInteger(Generator.GenerateBits(89, seed == -1 ? -1 : seed + 2)),
                new BigInteger(Generator.GenerateBits(89, seed == -1 ? -1 : seed + 3))
            };

            Func<ulong, BigInteger> g = HashFunction.kIndependentMultiplyModPrime(a);
            Tuple<Func<ulong, ulong>, Func<ulong, int>> hAnds = HashFunction.CountSketchHashfunctions(g, t);
            this.h = hAnds.Item1;
            this.s = hAnds.Item2;
            C = new long[1 << t];
        }

        public void Process(Tuple<ulong, long> entry)
        {
            C[h(entry.Item1)] += s(entry.Item1) * entry.Item2;
        }

        public long Query(ulong x)
        {
            return s(x) * C[x];
        }
    }

    public class FullCountSketch
    {
        List<BasicCountSketch> sketches = null;

        public FullCountSketch(int m, int t, int seed = -1)
        {
            for (int i = 0; i < m; i++)
                sketches.Add(new BasicCountSketch(t, seed == -1 ? -1 : seed + i * 4));
        }

        public void Process(Tuple<ulong, long> entry)
        {
            foreach (BasicCountSketch bcs in sketches)
                bcs.Process(entry);
        }

        //Get median by ordering query results and then taking the middle elements
        public long Query(ulong x)
        {
            return sketches.Select(bcs => bcs.Query(x)).OrderBy(val => val).ElementAt(sketches.Count / 2);
        }
    }
}
