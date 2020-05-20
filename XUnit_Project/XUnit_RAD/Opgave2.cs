using RADImplementationProject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public void TestHashTableSimpleSmall()
        {
            int n = 100000;

            //Generate 10 elements with different value for l
            //n < 2^l, n < 2^40
            IEnumerable<Tuple<ulong, int>> S = Generator.CreateStream(n, 63, 1);

            //Maps all keys from S to [0,15]
            HashTableChaining<int> table = new HashTableChaining<int>(7);

            foreach(Tuple<ulong, int> elem in S)
            {
                table.set(elem.Item1, (Number)elem.Item2);
            }

            foreach (Tuple<ulong, int> elem in S)
            {
                Assert.Equal(elem.Item2, table.get(elem.Item1));
            }

            output.WriteLine("Streamsize: " + n);
            output.WriteLine("Our hashtable size: " + table.Count);
            output.WriteLine(table.ToString());
        }
    }
}
