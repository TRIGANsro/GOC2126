using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kalkulator
{
    internal class Prvocisla
    {
        private readonly List<int> prvocisla = new List<int>();

        const int MAX = 1000000;

        internal Prvocisla()
        {
            for (int i = 2; i <= MAX; i++)
            {
                bool jePrvocislo = true;
                for (int j = 2; j <= Math.Sqrt(i); j++)
                {
                    if (i % j == 0)
                    {
                        jePrvocislo = false;
                        break;
                    }
                }
                if (jePrvocislo)
                {
                    prvocisla.Add(i);
                }
            }
        }

        internal bool CheckPrvocisla(int cislo)
        {
            if (cislo < 2)
            {
                return false;
            }
            if (cislo > MAX)
            {
                throw new ArgumentOutOfRangeException(nameof(cislo),"Zadané číslo je příliš velké.");
            }
            return prvocisla.Contains(cislo);
        }
    }
}
