using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace RADImplementationProject
{
    public class Generator
    {
        /// <summary>
        /// Generere n tilfældige bits i et byte array (der tages højde for bits der ikke går op i 8)
        /// </summary>
        /// <param name="n">Altalet af bits</param>
        /// <param name="seed">Seed anvendt til tilfældighedsgenerering, samme seed giver samme resultat</param>
        /// <returns></returns>
        public static byte[] GenerateBits(int n, int seed = -1)
        {
            byte[] bytes = new byte[(n + 7) / 8];

            Random random = new Random();
            if (seed != -1)
                random = new Random(seed);

            random.NextBytes(bytes);
            int rshifts = 8 - n % 8;

            bytes[bytes.Length - 1] = (byte) (bytes[bytes.Length - 1] >> rshifts);
            return bytes;
        }

        /// <summary>
        /// Sætter den mindst "significant" bit i vores byte array til 1 hvis den ellers ikke er det
        /// </summary>
        /// <param name="input">Byte array, som repræsentere et tal, hvor vi ønsker det skal være ulige</param>
        /// <returns>Det ulige byte array</returns>
        public static byte[] MakeOdd(byte[] input)
        {
            input[input.Length - 1] = (byte) ((input[^1] & 1) == 1 ? input[input.Length - 1] : input[input.Length - 1] + 1);
            return input;
        }

        /// <summary>
        /// Udregning af den aktuelle 2nd moment sum i en hashliste
        /// </summary>
        /// <typeparam name="T">Typen anvendt i hashtabellen</typeparam>
        /// <param name="hashTable">Hashtabellen</param>
        /// <returns>2nd moment summen over alle elementer i "hashTable"</returns>
        public static BigInteger RealCount<T>(HashTableChaining<T> hashTable)
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

        // Stream generatoren fra opgaven
        public static IEnumerable<Tuple<ulong, int>> CreateStream(int n, int l = 7, int seed = -1)
        {
            // We generate a random uint64 number.
            Random rnd = new Random();
            if (seed != -1)
                rnd = new Random(seed);
            ulong a = 0UL;
            Byte[] b = new Byte[8];
            rnd.NextBytes(b);

            for (int i = 0; i < 8; i++)
                a = (a << 8) + (ulong)b[i];

            // We demand that our random number has 30 zeros on the least significant bits and then one

            a = (a | ((1UL << 31) - 1UL)) ^ ((1UL << 30) - 1UL);

            ulong x = 0UL;

            for (int i = 0; i < n / 3; i++)
            {
                x = x + a;
                yield return Tuple.Create(x & (((1UL << l) - 1UL) << 30), 1);
            }

            for (int i = 0; i < (n + 1) / 3; i++)
            {
                x = x + a;
                yield return Tuple.Create(x & (((1UL << l) - 1UL) << 30), -1);
            }

            for (int i = 0; i < (n + 2) / 3; i++)
            {
                x = x + a;
                yield return Tuple.Create(x & (((1UL << l) - 1UL) << 30), 1);
            }
        }

        // Variant af stream generatoren fra opgaven, dette gøres for at undgå overflow og unødig casting når vi udregner 2nd moment sum
        public static IEnumerable<Tuple<ulong, long>> CreateStreamLong(int n, int l = 7, int seed = -1)
        {
            // We generate a random uint64 number.
            Random rnd = new Random();
            if (seed != -1)
                rnd = new Random(seed);
            ulong a = 0UL;
            Byte[] b = new Byte[8];
            rnd.NextBytes(b);

            for (int i = 0; i < 8; i++)
                a = (a << 8) + (ulong)b[i];

            // We demand that our random number has 30 zeros on the least significant bits and then one

            a = (a | ((1UL << 31) - 1UL)) ^ ((1UL << 30) - 1UL);

            ulong x = 0UL;

            for (int i = 0; i < n / 3; i++)
            {
                x = x + a;
                yield return Tuple.Create(x & (((1UL << l) - 1UL) << 30), 1L);
            }

            for (int i = 0; i < (n + 1) / 3; i++)
            {
                x = x + a;
                yield return Tuple.Create(x & (((1UL << l) - 1UL) << 30), -1L);
            }

            for (int i = 0; i < (n + 2) / 3; i++)
            {
                x = x + a;
                yield return Tuple.Create(x & (((1UL << l) - 1UL) << 30), 1L);
            }
        }
    }
}
