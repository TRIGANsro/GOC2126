// File: FastCompare.cs
// Usage: dotnet run -- A.txt B.txt [chunkMB=16] [maxSamples=100]
//
// - Paralelní čtení přes RandomAccess.Read (dotnet 8)
// - Vektorové porovnání přes System.Numerics.Vector<byte>
// - Vypíše souhrn a prvních N offsetů rozdílů (pro ukázku zrychlení)

using System;
using System.Buffers;
using System.IO;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

class Program
{
    static int Main(string[] args)
    {
        string pathA = args.Length > 0 ? args[0] : "A.txt";
        string pathB = args.Length > 1 ? args[1] : "B.txt";
        
        int chunkMb = args.Length > 2 && int.TryParse(args[2], out var cm) ? Math.Max(4, cm) : 16;
        int maxSamples = args.Length > 3 && int.TryParse(args[3], out var ms) ? Math.Max(0, ms) : 100;

        using var fa = System.IO.File.OpenHandle(pathA, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var fb = System.IO.File.OpenHandle(pathB, FileMode.Open, FileAccess.Read, FileShare.Read);

        long lenA = RandomAccess.GetLength(fa);
        long lenB = RandomAccess.GetLength(fb);
        if (lenA != lenB)
        {
            Console.WriteLine($"Různé délky: A={lenA} B={lenB} (minimálně {Math.Abs(lenA - lenB)} rozdílů).");
        }
        long length = Math.Min(lenA, lenB);

        int chunkSize = chunkMb * 1024 * 1024;
        int vecSize = Vector<byte>.Count;

        // Rozdělíme rozsah do rovnoměrných bloků
        int blockCount = (int)Math.Ceiling((double)length / chunkSize);

        long totalDiffs = 0;
        object lockObj = new();
        var samples = new System.Collections.Generic.List<long>(maxSamples);

        // Paralelně přes bloky
        Parallel.For(0, blockCount, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, blockIdx =>
        {
            long offset = (long)blockIdx * chunkSize;
            int size = (int)Math.Min(chunkSize, length - offset);

            // Půjčíme si buffery
            byte[] bufA = ArrayPool<byte>.Shared.Rent(size);
            byte[] bufB = ArrayPool<byte>.Shared.Rent(size);

            try
            {
                int readA = RandomAccess.Read(fa, bufA.AsSpan(0, size), offset);
                int readB = RandomAccess.Read(fb, bufB.AsSpan(0, size), offset);
                if (readA != readB)
                {
                    // Je to krajní blok na konci – přizpůsobíme délku
                    int m = Math.Min(readA, readB);
                    readA = readB = m;
                }

                var spanA = new ReadOnlySpan<byte>(bufA, 0, readA);
                var spanB = new ReadOnlySpan<byte>(bufB, 0, readB);

                long localDiffs = 0;

                // Vektorová část
                int i = 0;
                if (readA >= vecSize)
                {
                    int limit = readA - (readA % vecSize);
                    while (i < limit)
                    {
                        var va = new Vector<byte>(spanA.Slice(i, vecSize));
                        var vb = new Vector<byte>(spanB.Slice(i, vecSize));
                        var cmp = Vector.Equals(va, vb); // maska
                        if (Vector<byte>.Zero.Equals(cmp)) // nic se nerovná? (tj. všechno různé) – vzácné
                        {
                            localDiffs += vecSize;
                            if (maxSamples > 0)
                            {
                                lock (lockObj)
                                {
                                    for (int k = 0; k < vecSize && samples.Count < maxSamples; k++)
                                        samples.Add(offset + i + k);
                                }
                            }
                        }
                        else if (!Vector<byte>.One.Equals(cmp)) // je tam mix
                        {
                            // Propad na sken prvků (jen v rámci vektoru)
                            for (int k = 0; k < vecSize; k++)
                            {
                                if (spanA[i + k] != spanB[i + k])
                                {
                                    localDiffs++;
                                    if (maxSamples > 0)
                                    {
                                        lock (lockObj)
                                        {
                                            if (samples.Count < maxSamples) samples.Add(offset + i + k);
                                        }
                                    }
                                }
                            }
                        }
                        // else: vše shodné
                        i += vecSize;
                    }
                }

                // Zbytek (skalárně)
                for (; i < readA; i++)
                {
                    if (spanA[i] != spanB[i])
                    {
                        localDiffs++;
                        if (maxSamples > 0)
                        {
                            lock (lockObj)
                            {
                                if (samples.Count < maxSamples) samples.Add(offset + i);
                            }
                        }
                    }
                }

                System.Threading.Interlocked.Add(ref totalDiffs, localDiffs);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(bufA);
                ArrayPool<byte>.Shared.Return(bufB);
            }
        });

        Console.WriteLine($"Hotovo. Celkový počet odlišných bajtů: {totalDiffs}");
        if (samples.Count > 0)
        {
            samples.Sort();
            Console.WriteLine($"Prvních {samples.Count} offsetů s rozdíly (v bajtech od začátku):");
            foreach (var s in samples)
                Console.WriteLine(s);
        }

        if (lenA != lenB) Console.WriteLine($"Pozn.: Soubory mají rozdílnou délku, minimálně {Math.Abs(lenA - lenB)} dalších rozdílů mimo společný rozsah.");
        Console.WriteLine();
        Console.WriteLine("Tipy k dalšímu zrychlení pro studenty:");
        Console.WriteLine("  • Přemapovat offset → (řádek, sloupec) pomocí předpočtených pozic '\\n' a binárního vyhledávání.");
        Console.WriteLine("  • Vyzkoušet větší chunk (32–128 MB) dle disku a RAM.");
        Console.WriteLine("  • Přidat AVX2 intrinsics (System.Runtime.Intrinsics) pro 32B/64B kroky.");
        Console.WriteLine("  • Zkusit MemoryMappedFile pro snazší práci s velmi velkými soubory.");
        Console.WriteLine("  • Omezit synchronizaci (lock) – sbírat vzorky per-thread a slévat až na konci.");
        return 0;
    }
}
