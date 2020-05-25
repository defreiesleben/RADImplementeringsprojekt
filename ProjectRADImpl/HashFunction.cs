using System;
using System.Numerics;

namespace RADImplementationProject
{
    public class HashFunction
    {
        // Opgave 1 (a)
        // I opgave 1(a) bliver det understreget at I ikke skal bruge bigint eller BigInteger ved implementering af multiply-shift.
        public static Func<ulong, ulong> MultiplyShift(ulong a, int l = 7)
        {
            return new Func<ulong, ulong>(x => ((a * x) >> (64 - l)));
        }

        // Opgave 1 (b)
        public static Func<ulong, ulong> MultiplyModPrime(BigInteger a, BigInteger b, int l = 7, int q = 89)
        {
            BigInteger p = ((BigInteger)1 << q) - 1;
            BigInteger m = ((BigInteger)1 << l) - 1;
            if (l < 64 && a < p && b < p)
            {
                return new Func<ulong, ulong>(x =>
                {
                    // Det fungerer her med bitwise & pga strukturen på bits.
                    // Vi kan gøre dette med Mersenne Primes, da 2^q - 1 giver q - 1 bits = 1.
                    // Hvis man anvender en &-operator på ovenstående, vil man så få % p.
                    //Console.WriteLine("x = " + x + " and mapping to 0,...," + ((BigInteger)1 << l));
                    BigInteger res = ((((a * x) + b) & p) + (((a * x) + b) >> q));
                    //Console.WriteLine("fst step y = " + ((((BigInteger)a * x) + b) & p) + " + " + ((((BigInteger)a * x) + b) >> q) + " = " + res);

                    // The following, due to exercise 2.8 in assignment 3, "pulls" the result down to me mod p
                    // as x mod 2^q + \floor{x / 2^q} = x - \floor{x / 2^q}*p
                    while (res >= p)
                        res -= p;

                    //Console.WriteLine("snd step y = " + res + " % " + (2 ^ l));

                    return (ulong) (res & m);
                });
            }
            else
                throw new Exception("Some parameter was invalid");
        }

        // The function below is to assert to the MultiplyModPrime
        public static Func<ulong, ulong> MultiplyModPrimeReal(BigInteger a, BigInteger b, int l = 7, int q = 89)
        {
            BigInteger p = ((BigInteger)1 << q) - 1;
            BigInteger m = ((BigInteger)1 << l);
            if (l < 64 && a < p && b < p) {
                return new Func<ulong, ulong>(x =>
                {
                    // Det fungerer her med bitwise & pga strukturen på bits.
                    // Vi kan gøre dette med Mersenne Primes, da 2^q - 1 giver q - 1 bits = 1.
                    // Hvis man anvender en &-operator på ovenstående, vil man så få % p.
                    BigInteger res = ((a * x) + b) % p;
                    //Console.WriteLine("snd step y = " + res + " % " + (2 ^ l));

                    return (ulong) (res % m);
                });
            }
            else
                throw new Exception("Some parameter was invalid");
        }

        //Opgave 4
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
                if (y >= p) y = y - p; // h:U -> [m]    h:U -> [p]
                return y;
            });
        }

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

        // Opgave 1 (c)
        // I skal nu teste køretiderne af de to hashfunktioner I har implementeret. Brug generatoren i 1.2 til at generere en strøm af nøgler og
        // udskriv summen af deres hashværdier \sum_{i = 1}^{n} h(x), både for MultiplyModPrime og MultiplyShift-funktionerne


        // Kan dette gøres i XUnit-testenen?
        // Det er vel bare at summere res og printe resultatet
        // Jeg antager at summen af MultiplyModPrime er langt lavere end MultiplyShift
        // Det skal køres i samme loop - Skal der være to forskellige a-værdier?
        // Rapporter køretiderne og kommenter på de forskelle I ser.










        // Opgave 2 (a)
        // get(x): Skal returnere den værdi, der tilhører nøglen x. Hvis x ikke er i tabellen skal der returneres 0



        // Opgave 2 (b)
        // set(x, v): Skal sætte nøglen x til at have værdien v. Hvis x ikke allerede er i tabellen så tilføjes den til tabellen med værdien v

        // Opgave 2 (c)
        // increment(x, d): Skal lægge d til værdien tilhørende x. Hvis x ikke er i tabellen, skal x tilføjes til tabellen med værdien d.
        // Denne bliver lidt tricky, da vi skal "unhashe" værdierne or at se om x er der?



        // Opgave 3
        // Implementer en funktion der givet en strøm 􏰀2 af par(x1, d1),...,(xn, dn) beregner kvadratsummen S = \sum x∈U s(x)^2 .
        // I skal bruge hashta- bellen som I har implementeret i opgave 2 til at gemme værdierne for hvert x i strømmen.
        // I opgave 3 bliver I bedt om også at rapportere det mindste l så det med 2^l forskellige nøgler bliver
        // for ressourcekrævende at køre hashing med chaining.

    }
}
