using RADImplementationProject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
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
        public void Test100BCSResultsParallel()
        {
            int seed = 1;
            int n = 100000; //100000
            int l = 17; //2^17 ~ 131000
            int t_min = 1;
            int t_max = 17; //fails due to space on my machine at 29

            IEnumerable<Tuple<ulong, long>> S = Generator.CreateStreamLong(n, l, seed);


            BigInteger[] a_s = new BigInteger[]
            {
                new BigInteger(Generator.GenerateBits(89, 1)),
                new BigInteger(Generator.GenerateBits(89, 2)),
                new BigInteger(Generator.GenerateBits(89, 3)),
                new BigInteger(Generator.GenerateBits(89, 4))
            };

            HashTableChaining<long> table4MMP = new HashTableChaining<long>(
                HashFunction.CountSketchHashfunctions(HashFunction.kIndependentMultiplyModPrime(a_s), t_max).Item1, 
                1UL << t_max
            );


            Stopwatch sw = new Stopwatch();
            sw.Restart();
            foreach (Tuple<ulong, long> elem in S)
                table4MMP.increment(elem.Item1, (NumberLong)elem.Item2);
           
            BigInteger Real_S = Generator.RealCount<long>(table4MMP);
            sw.Stop();
            long Real_S_time = sw.ElapsedMilliseconds;


            string path = Directory.GetCurrentDirectory();
            DirectoryInfo di = new DirectoryInfo(path);
            while (di.Name != "XUnit_RAD")
                di = di.Parent;
            if (!Directory.Exists(Path.Combine(di.FullName, "TestResult")))
                Directory.CreateDirectory(Path.Combine(di.FullName, "TestResult"));

            string testfilePath = Path.Combine(di.FullName, "TestResult", "Test100BCSResultsParallel.csv");

            if (File.Exists(testfilePath))
                File.Delete(testfilePath);
            File.Create(testfilePath).Close();

            for (int t = t_min; t <= t_max; t++)
            {
                BigInteger[][] unsortedResults = new BigInteger[10][];

                for (int run = 0; run < 10; run++)
                {
                    Task<BigInteger>[] resultTasks = new Task<BigInteger>[100];
                    sw.Restart();
                    for (int i = 0; i < 100; i++)
                    {
                        resultTasks[i] = Task<BigInteger>.Factory.StartNew((object obj) =>
                        {
                            IEnumerable<Tuple<ulong, long>> S = (IEnumerable<Tuple<ulong, long>>)obj;
                            BasicCountSketch bcs = new BasicCountSketch(t);
                            foreach (Tuple<ulong, long> elem in S)
                                bcs.Process(elem);
                            return bcs.Estimate2ndMoment();
                        }, S);
                    }
                    sw.Stop();
                    Task<BigInteger>.WaitAll(resultTasks);
                    BigInteger[] results = resultTasks.Select(task => task.Result).ToArray();
                    unsortedResults[run] = results;
                }
                long t_iteration_time = sw.ElapsedMilliseconds;

                BigInteger[][] manyResults = new BigInteger[10][];
                BigInteger[][] manyMedianResults = new BigInteger[10][];
                BigInteger[] meansquare = new BigInteger[10];
                for (int run = 0; run < 10; run++)
                {
                    manyResults[run] = unsortedResults[run].OrderBy(val => val).ToArray();

                    meansquare[run] = unsortedResults[run].Aggregate(BigInteger.Zero, (acc, x) => acc + (BigInteger)Math.Pow((long)x - (long)Real_S, 2)) / 100;

                    BigInteger[] medianResults = new BigInteger[9];
                    for (int i = 0; i < 9; i++)
                    {
                        medianResults[i] = unsortedResults[run].Skip(i * 11).Take(11).OrderBy(val => val).ElementAt(5);
                    }
                    manyMedianResults[run] = medianResults.OrderBy(val => val).ToArray();
                }

                StringBuilder sb = new StringBuilder();

                sb.AppendLine("Real S;" + Real_S + ";Expectation;" + Real_S + ";Variance;" + Math.Round(2 * Math.Pow((long)Real_S, 2) / (1 << t), 2) + ";Value of t;" + t + ";Value of m;" + (1 << t) + ";Time taken;" + t_iteration_time + ";");
                sb.Append("MeanSquareErrors");
                BigInteger average = 0;
                for (int run = 0; run < 10; run++)
                {
                    sb.Append(";" + meansquare[run]);
                    average += meansquare[run];
                }
                sb.Append(";" + average / 10);
                sb.Append(";Real S time;" + Real_S_time);
                sb.AppendLine();

                sb.AppendLine("#Estimate;1st run;2nd run;3rd run;4th run;5th run;6th run;7th run;8th run;9th run;10th run;Average run;Real S");
                for (int i = 0; i < 100; i++)
                {
                    sb.Append(i + 1);
                    average = 0;
                    for (int run = 0; run < 10; run++)
                    {
                        sb.Append(";" + manyResults[run][i]);
                        average += manyResults[run][i];
                    }
                    sb.Append(";" + average / 10);
                    sb.Append(";" + Real_S);
                    sb.AppendLine();
                }

                sb.AppendLine("#Estimate;1st run;2nd run;3rd run;4th run;5th run;6th run;7th run;8th run;9th run;10th run;Average run;Real S");
                for (int i = 0; i < 9; i++)
                {
                    sb.Append(i + 1);
                    average = 0;
                    for (int run = 0; run < 10; run++)
                    {
                        sb.Append(";" + manyMedianResults[run][i]);
                        average += manyMedianResults[run][i];
                    }
                    sb.Append(";" + average / 10);
                    sb.Append(";" + Real_S);
                    sb.AppendLine();
                }
                sb.AppendLine(";;;;;;;;;;;;");

                File.AppendAllText(testfilePath, sb.ToString());
            }
        }

        [Fact]
        public void Test100BCSResultsSquential()
        {
            int seed = 1;
            int n = 100000; //100000
            int l = 17; //2^17 ~ 131000
            int t_min = 1;
            int t_max = 17; //fails due to space on my machine at 29

            IEnumerable<Tuple<ulong, long>> S = Generator.CreateStreamLong(n, l, seed);


            BigInteger[] a_s = new BigInteger[]
            {
                new BigInteger(Generator.GenerateBits(89, 1)),
                new BigInteger(Generator.GenerateBits(89, 2)),
                new BigInteger(Generator.GenerateBits(89, 3)),
                new BigInteger(Generator.GenerateBits(89, 4))
            };

            HashTableChaining<long> table4MMP = new HashTableChaining<long>(
                HashFunction.CountSketchHashfunctions(HashFunction.kIndependentMultiplyModPrime(a_s), t_max).Item1,
                1UL << t_max
            );


            Stopwatch sw = new Stopwatch();
            foreach (Tuple<ulong, long> elem in S)
                table4MMP.increment(elem.Item1, (NumberLong)elem.Item2);

            sw.Restart();
            BigInteger Real_S = Generator.RealCount<long>(table4MMP);
            sw.Stop();
            long Real_S_time = sw.ElapsedMilliseconds;


            string path = Directory.GetCurrentDirectory();
            DirectoryInfo di = new DirectoryInfo(path);
            while (di.Name != "XUnit_RAD")
                di = di.Parent;
            if (!Directory.Exists(Path.Combine(di.FullName, "TestResult")))
                Directory.CreateDirectory(Path.Combine(di.FullName, "TestResult"));

            string testfilePath = Path.Combine(di.FullName, "TestResult", "Test100BCSResultsSquential.csv");

            if (File.Exists(testfilePath))
                File.Delete(testfilePath);
            File.Create(testfilePath).Close();

            for (int t = t_min; t <= t_max; t++)
            {
                BigInteger[][] unsortedResults = new BigInteger[10][];

                for (int run = 0; run < 10; run++)
                {
                    BigInteger[] results = new BigInteger[100];
                    sw.Restart();
                    for (int i = 0; i < 100; i++)
                    {
                        sw.Stop();
                        BasicCountSketch bcs = new BasicCountSketch(t);
                        foreach (Tuple<ulong, long> elem in S)
                            bcs.Process(elem);
                        sw.Start();
                        results[i] = bcs.Estimate2ndMoment();
                    }
                    sw.Stop();
                    unsortedResults[run] = results;
                }
                long t_iteration_time = sw.ElapsedMilliseconds;

                BigInteger[][] manyResults = new BigInteger[10][];
                BigInteger[][] manyMedianResults = new BigInteger[10][];
                BigInteger[] meansquare = new BigInteger[10];
                for (int run = 0; run < 10; run++)
                {
                    manyResults[run] = unsortedResults[run].OrderBy(val => val).ToArray();

                    meansquare[run] = unsortedResults[run].Aggregate(BigInteger.Zero, (acc, x) => acc + (BigInteger)Math.Pow((long)x - (long)Real_S, 2)) / 100;

                    BigInteger[] medianResults = new BigInteger[9];
                    for (int i = 0; i < 9; i++)
                    {
                        medianResults[i] = unsortedResults[run].Skip(i * 11).Take(11).OrderBy(val => val).ElementAt(5);
                    }
                    manyMedianResults[run] = medianResults.OrderBy(val => val).ToArray();
                }

                StringBuilder sb = new StringBuilder();

                sb.AppendLine("Real S;" + Real_S + ";Expectation;" + Real_S + ";Variance;" + Math.Round(2 * Math.Pow((long)Real_S, 2) / (1 << t), 2) + ";Value of t;" + t + ";Value of m;" + (1 << t) + ";Time taken;" + t_iteration_time + ";");
                sb.Append("MeanSquareErrors");
                BigInteger average = 0;
                for (int run = 0; run < 10; run++)
                {
                    sb.Append(";" + meansquare[run]);
                    average += meansquare[run];
                }
                sb.Append(";" + average / 10);
                sb.Append(";Real S time;" + Real_S_time);
                sb.AppendLine();

                sb.AppendLine("#Estimate;1st run;2nd run;3rd run;4th run;5th run;6th run;7th run;8th run;9th run;10th run;Average run;Real S");
                for (int i = 0; i < 100; i++)
                {
                    sb.Append(i + 1);
                    average = 0;
                    for (int run = 0; run < 10; run++)
                    {
                        sb.Append(";" + manyResults[run][i]);
                        average += manyResults[run][i];
                    }
                    sb.Append(";" + average / 10);
                    sb.Append(";" + Real_S);
                    sb.AppendLine();
                }

                sb.AppendLine("#Estimate;1st run;2nd run;3rd run;4th run;5th run;6th run;7th run;8th run;9th run;10th run;Average run;Real S");
                for (int i = 0; i < 9; i++)
                {
                    sb.Append(i + 1);
                    average = 0;
                    for (int run = 0; run < 10; run++)
                    {
                        sb.Append(";" + manyMedianResults[run][i]);
                        average += manyMedianResults[run][i];
                    }
                    sb.Append(";" + average / 10);
                    sb.Append(";" + Real_S);
                    sb.AppendLine();
                }
                sb.AppendLine(";;;;;;;;;;;;");

                File.AppendAllText(testfilePath, sb.ToString());
            }
        }

        [Fact]
        public void Test100BCSResultsReduced() //takes too long...
        {
            int seed = 1;
            int n = 100000; //100000
            int l = 7;
            int t_min = 29;
            int t_max = 31;

            IEnumerable<Tuple<ulong, long>> S = Generator.CreateStreamLong(n, l, seed);

            HashTableChaining<long> tableMS = new HashTableChaining<long>(
                HashFunction.MultiplyShift(BitConverter.ToUInt64(Generator.MakeOdd(Generator.GenerateBits(64, seed))), l),
                1UL << l
            );
            foreach (Tuple<ulong, long> elem in S)
                tableMS.increment(elem.Item1, (NumberLong)elem.Item2);

            
            BigInteger Real_S = Generator.RealCount<long>(tableMS);

            string path = Directory.GetCurrentDirectory();
            DirectoryInfo di = new DirectoryInfo(path);
            while (di.Name != "XUnit_RAD")
                di = di.Parent;
            if (!Directory.Exists(Path.Combine(di.FullName, "TestResult")))
                Directory.CreateDirectory(Path.Combine(di.FullName, "TestResult"));

            string testfilePath = Path.Combine(di.FullName, "TestResult", "Test100BCSResultsReduced.csv");

            if (File.Exists(testfilePath))
                File.Delete(testfilePath);
            File.Create(testfilePath).Close();

            for (int t = t_min; t <= t_max; t++)
            {
                BigInteger[] unsortedResults = new BigInteger[100];
                for (int i = 0; i < 100; i++)
                {
                    BasicCountSketch bcs = new BasicCountSketch(t);
                    foreach (Tuple<ulong, long> elem in S)
                        bcs.Process(elem);
                    unsortedResults[i] = bcs.Estimate2ndMoment();
                }

                BigInteger[] manyResults = unsortedResults.OrderBy(val => val).ToArray();
                BigInteger meansquare = unsortedResults.Aggregate(BigInteger.Zero, (acc, x) => acc + (BigInteger)Math.Pow((long)x - (long)Real_S, 2)) / 100;

                BigInteger[] medianResults = new BigInteger[9];
                for (int i = 0; i < 9; i++)
                {
                    medianResults[i] = unsortedResults.Skip(i * 11).Take(11).OrderBy(val => val).ElementAt(5);
                }
                medianResults = medianResults.OrderBy(val => val).ToArray();

                StringBuilder sb = new StringBuilder();

                sb.AppendLine("Real S;" + Real_S + ";Expectation;" + Real_S + "\nVariance;" + Math.Round(2 * Math.Pow((long)Real_S, 2) / (1 << t), 2) + ";Value of t;" + t + "\nValue of m;" + (1 << t) + ";;");
                sb.Append("MeanSquareErrors");
                BigInteger average = 0;
                sb.Append(";" + meansquare);
                average += meansquare;
                sb.Append(";" + average / 10);
                sb.AppendLine();

                sb.AppendLine("#Estimate;1st run;Average run;Real S");
                for (int i = 0; i < 100; i++)
                {
                    sb.Append(i + 1);
                    average = 0;
                    sb.Append(";" + manyResults[i]);
                    average += manyResults[i];
                    sb.Append(";" + average / 10);
                    sb.Append(";" + Real_S);
                    sb.AppendLine();
                }

                sb.AppendLine("#Estimate;1st run;Average run;Real S");
                for (int i = 0; i < 9; i++)
                {
                    sb.Append(i + 1);
                    average = 0;
                    sb.Append(";" + medianResults[i]);
                    average += medianResults[i];
                    sb.Append(";" + average / 10);
                    sb.Append(";" + Real_S);
                    sb.AppendLine();
                }
                sb.AppendLine(";;;");

                File.AppendAllText(testfilePath, sb.ToString());
            }
        }
    }
}
