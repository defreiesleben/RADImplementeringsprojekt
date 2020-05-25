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

        public BigInteger RealCount<T>(HashTableChaining<T> hashTable)
        {
            BigInteger sum_squared = BigInteger.Zero;
            foreach (Node<T> node in hashTable.NodeList)
            {
                Node<T> head = node;
                while (head != null && head.Data is NumberLong val)
                {
                    sum_squared += (long)Math.Pow(val.GetValue(), 2);
                    head = head.Next;
                }
            }
            return sum_squared;
        }

        [Fact]
        public void TestEstimationOfS()
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

                BigInteger Real_S_MS = RealCount<long>(tableMS);
                BigInteger Real_S_MMP = RealCount<long>(tableMMP);
                sw.Stop();

                //output.WriteLine("MS table: \n " + tableMS);
                //output.WriteLine("MMP table: \n " + tableMMP);
                output.WriteLine("Iteration: {0,2}, time in ms: {1}", l, sw.ElapsedMilliseconds);
                output.WriteLine("\tMS table sum: " + Real_S_MS);
                output.WriteLine("\tMMP table sum: " + Real_S_MMP);
                l++;
            }
        }
    }
}
