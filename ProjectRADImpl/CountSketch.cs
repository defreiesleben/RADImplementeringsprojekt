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

            BigInteger[] a = new BigInteger[]
            {
                new BigInteger(Generator.GenerateBits(q, seed)),
                new BigInteger(Generator.GenerateBits(q, seed == -1 ? -1 : seed + 1)),
                new BigInteger(Generator.GenerateBits(q, seed == -1 ? -1 : seed + 2)),
                new BigInteger(Generator.GenerateBits(q, seed == -1 ? -1 : seed + 3))
            };

            Func<ulong, BigInteger> g = HashFunction.kIndependentMultiplyModPrime(a, q);
            Tuple<Func<ulong, ulong>, Func<ulong, int>> hAnds = HashFunction.CountSketchHashfunctions(g, t, q);
            this.h = hAnds.Item1;
            this.s = hAnds.Item2;
            this.C = new long[1 << t];
        }

        public void Process(Tuple<ulong, long> entry)
        {
            this.C[h(entry.Item1)] += this.s(entry.Item1) * entry.Item2;
        }

        public long Query(ulong x)
        {
            return this.s(x) * this.C[h(x)];
        }

        public BigInteger Estimate2ndMoment()
        {
            BigInteger ret = BigInteger.Zero;

            foreach (long c in this.C)
                ret += (long)Math.Pow(c, 2);

            return ret;
        }
    }

    public class FullCountSketch
    {
        List<BasicCountSketch> sketches = null;

        public FullCountSketch(int m, int t, int seed = -1)
        {
            for (int i = 0; i < m; i++)
                this.sketches.Add(new BasicCountSketch(t, seed == -1 ? -1 : seed + i * 4));
        }

        public void Process(Tuple<ulong, long> entry)
        {
            foreach (BasicCountSketch bcs in this.sketches)
                bcs.Process(entry);
        }

        //Get median by ordering query results and then taking the middle elements
        public long Query(ulong x)
        {
            IEnumerable<long> queryResults = this.sketches.Select(bcs => bcs.Query(x)).OrderBy(val => val);
            if ((this.sketches.Count & 1) == 1)
                return queryResults.ElementAt(this.sketches.Count >> 1);
            else
                return (queryResults.ElementAt(this.sketches.Count >> 1) + queryResults.ElementAt(this.sketches.Count >> 1 + 1)) >> 1;
        }

        //public BigInteger Estimate2ndMoment()
        //{
        //    BigInteger ret = BigInteger.Zero;
        //
        //    foreach (BasicCountSketch bcs in this.sketches)
        //        ret += (long)Math.Pow(bcs.Estimate2ndMoment(), 2);
        //
        //    return ret;
        //}
    }
}
