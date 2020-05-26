using System;
using System.Collections.Generic;
using System.Text;

namespace RADImplementationProject
{
    public class BasicCountSketch
    {
        private Func<ulong, ulong> h = null;
        private Func<ulong, int> s = null;

        private long[] C = null;

        public BasicCountSketch(Func<ulong, ulong> h, Func<ulong, int> s, int t)
        {
            if (t > 31)
                throw new Exception("t is used in creating C which has to be of max size 2^31, hence t <= 31");

            this.h = h;
            this.s = s;
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
}
