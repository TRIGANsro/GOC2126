// File: CompareFiles.cs
// Usage: dotnet run --project . -- A.txt B.txt
// Vypíše: řádek, sloupec (1-based), znak v A a znak v B.

using System;
using System.IO;
using System.Text;

class Program
{
    static readonly Encoding Enc = Encoding.ASCII;

    static void Main(string[] args)
    {
        Console.WriteLine(DateTime.Now);
        
        StringBuilder sb = new StringBuilder(100_000);
        
        string fileA = args.Length > 0 ? args[0] : "A.txt";
        string fileB = args.Length > 1 ? args[1] : "B.txt";

        using var ra = new StreamReader(new FileStream(fileA, FileMode.Open, FileAccess.Read, FileShare.Read), Enc);
        using var rb = new StreamReader(new FileStream(fileB, FileMode.Open, FileAccess.Read, FileShare.Read), Enc);

        long lineNo = 0;
        long totalDiffs = 0;

        while (true)
        {
            string? la = ra.ReadLine();
            string? lb = rb.ReadLine();

            if (la is null && lb is null) break;

            lineNo++;

            if (la is null)
            {
                sb.AppendLine($"Soubor A skončil na řádku {lineNo - 1}, B má další data (od řádku {lineNo}).");
                break;
            }
            if (lb is null)
            {
                sb.AppendLine($"Soubor B skončil na řádku {lineNo - 1}, A má další data (od řádku {lineNo}).");
                break;
            }

            int minLen = Math.Min(la.Length, lb.Length);
            for (int i = 0; i < minLen; i++)
            {
                if (la[i] != lb[i])
                {
                    totalDiffs++;
                    sb.AppendLine($"Řádek {lineNo}, sloupec {i + 1}: '{la[i]}' vs '{lb[i]}'");
                }
            }

            if (la.Length != lb.Length)
            {
                totalDiffs += Math.Abs(la.Length - lb.Length);
                sb.AppendLine(
                    $"Řádek {lineNo}: rozdílná délka (A={la.Length}, B={lb.Length}), " +
                    $"navíc {(la.Length > lb.Length ? "A" : "B")} má {Math.Abs(la.Length - lb.Length)} znaků.");
            }
        }

        sb.AppendLine($"Celkem rozdílů (včetně rozdílů délek): {totalDiffs}");
        Console.WriteLine(DateTime.Now);
        Console.WriteLine(sb.ToString());
    }
}
