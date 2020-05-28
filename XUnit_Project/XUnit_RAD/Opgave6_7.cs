using RADImplementationProject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace XUnit_RAD
{
    public class Opgave6_7
    {
        private readonly ITestOutputHelper output;
        public Opgave6_7(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Test100BCSResults()
        {
            int seed = 1;
            int n = 100000;
            int l = 7;
            int t = 4;

            IEnumerable<Tuple<ulong, long>> S = Generator.CreateStreamLong(n, l, seed);
            
            HashTableChaining<long> tableMS = new HashTableChaining<long>(
                HashFunction.MultiplyShift(BitConverter.ToUInt64(Generator.MakeOdd(Generator.GenerateBits(64, seed))), l), 
                1UL << l
            );
            foreach (Tuple<ulong, long> elem in S)
                tableMS.increment(elem.Item1, (NumberLong)elem.Item2);
            
            BigInteger Real_S = Generator.RealCount<long>(tableMS);


            BigInteger[][] manyResults = new BigInteger[10][];
            BigInteger[][] manyMedianResults = new BigInteger[10][];
            for (int run = 0; run < 10; run++)
            {
                BigInteger[] results = new BigInteger[100];
                for (int i = 0; i < 100; i++)
                {
                    BasicCountSketch bcs = new BasicCountSketch(4);

                    foreach (Tuple<ulong, long> elem in S)
                        bcs.Process(elem);

                    results[i] = bcs.Estimate2ndMoment();
                }
                manyResults[run] = results.OrderBy(val => val).ToArray();

                BigInteger[] medianResults = new BigInteger[9];
                for (int i = 0; i < 9; i++)
                {
                    medianResults[i] = results.Skip(i * 11).Take(11).OrderBy(val => val).ElementAt(5);
                }
                manyMedianResults[run] = medianResults.OrderBy(val => val).ToArray();
            }

            string path = Directory.GetCurrentDirectory();
            DirectoryInfo di = new DirectoryInfo(path);
            while (di.Name != "XUnit_RAD")
                di = di.Parent;
            if (!Directory.Exists(Path.Combine(di.FullName, "TestResult")))
                Directory.CreateDirectory(Path.Combine(di.FullName, "TestResult"));

            string testfilePath = Path.Combine(di.FullName, "TestResult", "Test100BCSResults.csv");

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Real S;" + Real_S + ";;;;;;;;;");
            sb.AppendLine("#Estimate;1st run;2nd run;3rd run;4th run;5th run;6th run;7th run;8th run;9th run;10th run");
            for (int i = 0; i < 100; i ++)
            {
                sb.Append(i + 1);
                for (int run = 0; run < 10; run++)
                    sb.Append(";" + manyResults[run][i]);
                sb.AppendLine();
            }

            sb.AppendLine("#Estimate;1st run;2nd run;3rd run;4th run;5th run;6th run;7th run;8th run;9th run;10th run"); 
            for (int i = 0; i < 9; i++)
            {
                sb.Append(i + 1);
                for (int run = 0; run < 10; run++)
                    sb.Append(";" + manyMedianResults[run][i]);
                sb.AppendLine();
            }

            if (!File.Exists(testfilePath))
                File.Create(testfilePath).Close();
            File.WriteAllText(testfilePath, sb.ToString());
        }
    }
}
