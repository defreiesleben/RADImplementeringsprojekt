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
    public class Opgave2
    {
        private readonly ITestOutputHelper output;
        public Opgave2(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void TestHashTableSimpleSmallMS()
        {
            int n = 100000;
            int seed = 1;

            //Generate 10 elements with different value for l
            IEnumerable<Tuple<ulong, int>> S = Generator.CreateStream(n, 63, 1);

            //Maps all keys from S to [0,127]
            ulong a = BitConverter.ToUInt64(Generator.MakeOdd(Generator.GenerateBits(64, seed)));
            int l = 7;
            HashTableChaining<int> table_MS = new HashTableChaining<int>(HashFunction.MultiplyShift(a, l), 1UL << l);

            foreach (Tuple<ulong, int> elem in S)
                table_MS.increment(elem.Item1, (Number)elem.Item2);

            foreach (Tuple<ulong, int> elem in S)
                Assert.Equal(elem.Item2, table_MS.get(elem.Item1));

            output.WriteLine("Streamsize: " + n);
            output.WriteLine("Our hashtable size: " + table_MS.Count);
            output.WriteLine(table_MS.ToString());
        }

        [Fact]
        public void TestHashTableSimpleSmallMMP()
        {
            int n = 100000;
            int l = 7;
            int seed = 1;

            //Generate 10 elements with different value for l
            IEnumerable<Tuple<ulong, int>> S = Generator.CreateStream(n, 63, seed);

            //Maps all keys from S to [0,127]
            BigInteger a = new BigInteger(Generator.GenerateBits(89, seed));
            BigInteger b = new BigInteger(Generator.GenerateBits(89, seed + 1));
            HashTableChaining<int> table_MMP = new HashTableChaining<int>(HashFunction.MultiplyModPrime(a, b, l), 1UL << l);

            foreach(Tuple<ulong, int> elem in S)
                table_MMP.increment(elem.Item1, (Number)elem.Item2);

            foreach (Tuple<ulong, int> elem in S)
                Assert.Equal(elem.Item2, table_MMP.get(elem.Item1));

            output.WriteLine("Streamsize: " + n);
            output.WriteLine("Our hashtable size: " + table_MMP.Count);
            output.WriteLine(table_MMP.ToString());
        }

        [Fact]
        public void TestStream()
        {
            int n = 32;

            //Generate 10 elements with different value for l
            IEnumerable<Tuple<ulong, int>> S = Generator.CreateStream(n, 2, 1);

            foreach(Tuple<ulong, int> entry in S)
            {
                output.WriteLine("Key: " + entry.Item1 + " Values: " + entry.Item2);
            }
        }
    }
}
