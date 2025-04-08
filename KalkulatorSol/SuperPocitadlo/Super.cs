using Kalkulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperPocitadlo;

public class Super
{
    public void Akce()
    {
        Pocitadlo pocitadlo = new Pocitadlo();

        int[] pole = new int[100_000];

        for (int i = 0; i < pole.Length; i++)
        {
            pole[i] = Random.Shared.Next(2,1_000_000);
        }

        int pocetPrvocisel = 0;
        string prvocisla = string.Empty;

        foreach (int cislo in pole)
        {
            if (pocitadlo.TestPrvocisla(cislo))
            {
                pocetPrvocisel++;
            }
            prvocisla += string.Format("Pocet prvocisel je {0}", pocetPrvocisel);
        }

        File.WriteAllText("prvocisla.txt", prvocisla);
    }
}
