using System;
using System.Numerics;

namespace RADImplementationProject
{
    public class HashFunctions
    {
        // Opgave 1 (a)
        public static BigInteger MultiplyShift(ulong x, int l = 7)
        {
            // a random generated 64-bit integer using www.random.org/bytes
            BigInteger a = 4272982255652177843UL;
            BigInteger res = (a * x) >> (64 - l);
            return res;
        }





        // Opgave 1 (b)
        public static BigInteger MultiplyModPrime(ulong x, int l = 7)
        {
            int q = 89;
            BigInteger a = new BigInteger(new byte[] { 164, 79, 14, 121, 180, 208, 203, 39, 15, 129, 113, 0 });
            BigInteger b = new BigInteger(new byte[] { 199, 179, 228, 108, 79, 196, 210, 181, 38, 194, 94, 0 });
            BigInteger p = ((BigInteger)1 << q) - 1;
            BigInteger m = ((BigInteger)1 << l) - 1;

            if (l < 64 && a < p && b < p)
            {
                // Det fungerer her med bitwise & pga strukturen på bits.
                // Vi kan gøre dette med Mersenne Primes, da 2^q - 1 giver q - 1 bits = 1.
                // Hvis man anvender en &-operator på ovenstående, vil man så få % p.
                //Console.WriteLine("x = " + x + " and mapping to 0,...," + ((BigInteger)1 << l));
                BigInteger res = (((((BigInteger)a * x) + b) & p) + ((((BigInteger)a * x) + b) >> q));
                //Console.WriteLine("fst step y = " + ((((BigInteger)a * x) + b) & p) + " + " + ((((BigInteger)a * x) + b) >> q) + " = " + res);

                // The following, due to exercise 2.8 in assignment 3, "pulls" the result down to me mod p
                // as x mod 2^q + \floor{x / 2^q} = x - \floor{x / 2^q}*p
                while (res >= p)
                    res -= p;

                //Console.WriteLine("snd step y = " + res + " % " + (2 ^ l));

                return res & m;

            }
            System.Console.WriteLine("Bitshift value above 64");
            return -1;
        }

        // The function below is to assert to the MultiplyModPrime
        public static BigInteger MultiplyModPrimeReal(ulong x, int l = 7)
        {
            int q = 89;
            BigInteger a = new BigInteger(new byte[] { 164, 79, 14, 121, 180, 208, 203, 39, 15, 129, 113, 0 });
            BigInteger b = new BigInteger(new byte[] { 199, 179, 228, 108, 79, 196, 210, 181, 38, 194, 94, 0 });
            BigInteger p = ((BigInteger)1 << q) - 1;
            BigInteger m = ((BigInteger)1 << l);

            if (l < 64 && a < p && b < p)
            {
                // Det fungerer her med bitwise & pga strukturen på bits.
                // Vi kan gøre dette med Mersenne Primes, da 2^q - 1 giver q - 1 bits = 1.
                // Hvis man anvender en &-operator på ovenstående, vil man så få % p.
                BigInteger res = (((BigInteger)a * x) + b) % p;
                //Console.WriteLine("snd step y = " + res + " % " + (2 ^ l));
                return res % m;
            }
            System.Console.WriteLine("Bitshift value above 64");
            return -1;
        }


        public static BigInteger hackModulo(int q, int x)
        {
            BigInteger p = ((BigInteger)1 << q) - 1;
            return x & p; // x mod 2^q
        }

        public static BigInteger normalModulo(int q, int x)
        {
            return x % ((BigInteger)1 << q); // x mod 2^q
        }

        public static BigInteger twothpower(int q)
        {
            return ((BigInteger)1 << q);
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
        // Implementer en funktion der givet en strøm 􏰀2 af par(x1, d1),...,(xn, dn) beregner kvadratsummen S = \sum x∈U s(x)^2 . I skal bruge hashta- bellen som I har implementeret i opgave 2 til at gemme værdierne for hvert x i strømmen.

    }
}
