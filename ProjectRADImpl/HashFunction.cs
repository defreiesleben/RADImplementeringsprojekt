using System;
using System.Numerics;

namespace RADImplementationProject
{
    public class HashFunction
    {
        // Opgave 1 (a)
        // I opgave 1(a) bliver det understreget at I ikke skal bruge bigint eller BigInteger ved implementering af multiply-shift.
        /// <param name="a">Hashfunktion konstant</param>
        /// <param name="l">Rummet vi mapper til m = [2^l]</param>
        /// <returns></returns>
        public static Func<ulong, ulong> MultiplyShift(ulong a, int l = 7)
        {
            return new Func<ulong, ulong>(x => ((a * x) >> (64 - l)));
        }

        // Opgave 1 (b)
        /// <param name="a">Hashfunktion konstant 1</param>
        /// <param name="b">Hashfunktion konstant 2</param>
        /// <param name="l">Rummet vi mapper til m = [2^l]</param>
        /// <param name="q">Mersenne prime (2^q - 1)</param>
        /// <returns>Hashfunktion der kan anvendes i notationen: h(x) -> hashed x </returns>
        public static Func<ulong, ulong> MultiplyModPrime(BigInteger a, BigInteger b, int l = 7, int q = 89)
        {
            BigInteger p = ((BigInteger)1 << q) - 1;
            BigInteger m = ((BigInteger)1 << l) - 1;
            if (l < 64 && a < p && b < p)
                return new Func<ulong, ulong>(x =>
                {
                    // Det fungerer her med bitwise & pga strukturen på bits.
                    // Vi kan gøre dette med Mersenne Primes, da 2^q - 1 giver q - 1 bits = 1.
                    // Hvis man anvender en &-operator på ovenstående, vil man så få % p.
                    BigInteger res = ((((a * x) + b) & p) + (((a * x) + b) >> q));

                    // The following, due to exercise 2.8 in assignment 3, "pulls" the result down to me mod p
                    // as x mod 2^q + \floor{x / 2^q} = x - \floor{x / 2^q}*p
                    while (res >= p)
                        res -= p;

                    return (ulong) (res & m);
                });
            else
                throw new Exception("Some parameter was invalid");
        }

        /// <summary>
        /// Denne funktion anvendes til at sammenligne med den øvrige "MultiplyModPrime" funktion
        /// og se om den giver de samme resultater som denne simplere version med modulus
        /// </summary>
        /// <param name="a">Hashfunktion konstant 1</param>
        /// <param name="b">Hashfunktion konstant 2</param>
        /// <param name="l">Rummet vi mapper til m = [2^l]</param>
        /// <param name="q">Mersenne prime (2^q - 1)</param>
        /// <returns>Hashfunktion der kan anvendes i notationen: h(x) -> hashed x </returns>
        public static Func<ulong, ulong> MultiplyModPrimeReal(BigInteger a, BigInteger b, int l = 7, int q = 89)
        {
            BigInteger p = ((BigInteger)1 << q) - 1;
            BigInteger m = ((BigInteger)1 << l);
            if (l < 64 && a < p && b < p)
                return new Func<ulong, ulong>(x =>
                {
                    BigInteger res = ((a * x) + b) % p;

                    return (ulong) (res % m);
                });
            else
                throw new Exception("Some parameter was invalid");
        }

        //Opgave 4
        /// <summary>
        /// Using Horner's rule we can make k dimensional multiply mod prime and the trick presented
        /// in 2moment-lect, then we can pretty much make any dimension
        /// </summary>
        /// <param name="a">Hashfunktion konstanter (1...)</param>
        /// <param name="q">Mersenne prime (2^q - 1)</param>
        /// <returns>Hashfunktion der kan anvendes i notationen: g(x) -> hashed x </returns>
        public static Func<ulong, BigInteger> kIndependentMultiplyModPrime(BigInteger[] a, int q = 89)
        {
            return new Func<ulong, BigInteger>(x =>
            {
                BigInteger y = a[a.Length - 1];
                BigInteger p = ((BigInteger)1 << q) - 1;
                for (int i = a.Length - 2; i >= 0; i--)
                {
                    y = y * x + a[i];
                    y = (y & p) + (y >> q);
                }
                if (y >= p) y = y - p;
                return y; // g:U -> [p]
            });
        }

        /// <summary>
        /// Creates the two hashfunctions used in count sketch
        /// </summary>
        /// <param name="g">The hashfunction provided by "kIndependentMultiplyModPrime"</param>
        /// <param name="t">The space to map down to (m = 2^t)</param>
        /// <param name="q">Mersenne prime (2^q - 1)</param>
        /// <returns>Returnere de to hashfunktioner til count sketch: h(x) |-> [m] and s(x) |-> [-1, 1]</returns>
        public static Tuple<Func<ulong, ulong>, Func<ulong, int>> CountSketchHashfunctions(Func<ulong, BigInteger> g, int t = 7, int q = 89)
        {
            if (t > 64 || t < 0)
                throw new Exception("invalid t given for countsketch functions, expected 0 <= t <= 64");

            Func<ulong, ulong> h = new Func<ulong, ulong>(x =>
            {
                return (ulong)(g(x) & ((1UL << t) - 1));
            });

            Func<ulong, int> s = new Func<ulong, int>(x =>
            {
                return 1 - ((int)(g(x) >> (q - 1)) << 1);
            });
            return Tuple.Create(h, s);
        }
    }
}
