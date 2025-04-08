using System.Numerics;

namespace Kalkulator;

public class Pocitadlo
{
    private static readonly List<Prvocisla> seznam = new();
    public int Secti(int a, int b) => a + b;
    public int Odecitani(int a, int b) => a - b;
    public int Nasobeni(int a, int b) => a * b;
    public int Deleni(int a, int b) => a / b;

    public int Mocnina(int a, int b) => (int)Math.Pow(a, b);
    public BigInteger Faktorial(int a)
    {
        if (a < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(a), "Faktoriál nelze počítat pro záporné číslo.");
        }
        if (a == 0)
        {
            return 1;
        }
        BigInteger vysledek = 1;
        for (int i = 1; i <= a; i++)
        {
            vysledek *= i;
        }
        return vysledek;
    }

    public bool TestPrvocisla(int cislo)
    {
        Prvocisla prvocisla = new Prvocisla();
        seznam.Add(prvocisla);
        return prvocisla.CheckPrvocisla(cislo);
    }
}
