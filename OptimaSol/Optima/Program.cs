// File: GenerateFiles.cs
// Usage: dotnet run --project . -- A.txt B.txt 4
// Poslední argument = velikost v GB (default 4)

using System;
using System.IO;
using System.Text;

class Program
{
    static readonly Encoding Enc = Encoding.ASCII;
    static readonly byte NewLine = (byte)'\n';
    static readonly char[] Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ".ToCharArray();

    static void Main(string[] args)
    {
        string fileA = args.Length > 0 ? args[0] : "A.txt";
        string fileB = args.Length > 1 ? args[1] : "B.txt";
        long sizeGb = args.Length > 2 && long.TryParse(args[2], out var g) ? g : 4;

        long targetBytes = sizeGb * 1024L * 1024L * 1024L;

        Console.WriteLine($"Generuji {fileA} (~{sizeGb} GB)...");
        GenerateBaseFile(fileA, targetBytes);

        Console.WriteLine($"Kopíruji do {fileB}...");
        if (File.Exists(fileB)) File.Delete(fileB);
        File.Copy(fileA, fileB);

        Console.WriteLine("Dělám 1000 náhodných jednopísmenných odchylek (mimo \\n) v B.txt...");
        IntroduceDifferences(fileB, 1000);

        Console.WriteLine("Hotovo.");
    }

    static void GenerateBaseFile(string path, long targetBytes)
    {
        // Jednoduché: náhodné řádky délky 80–120 znaků, ASCII, ukončené '\n'
        var rnd = new Random(12345);
        long written = 0;

        // Záměrně „prosté“ bufferované zápisy
        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
        using var sw = new StreamWriter(fs, Enc);

        while (written < targetBytes)
        {
            int len = rnd.Next(80, 121);
            var span = new char[len];
            for (int i = 0; i < len; i++)
                span[i] = Alphabet[rnd.Next(Alphabet.Length)];

            sw.Write(span);
            sw.Write('\n');

            // Kolik bajtů jsme přibližně přidali (ASCII -> 1B/char + 1B newline)
            written += len + 1;

            // (Neoptimalizujeme: žádné ruční flushe ani předpočty.)
        }

        sw.Flush();
    }

    static void IntroduceDifferences(string path, int differences)
    {
        var rnd = new Random(67890);

        using var fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        long length = fs.Length;

        int made = 0;
        var buffer = new byte[1];

        while (made < differences)
        {
            long pos = rnd.NextInt64(0, length); // .NET 8+
            fs.Position = pos;

            // Přečti bajt; pokud je newline, zkusíme jiné místo
            if (fs.Read(buffer, 0, 1) != 1) continue;
            byte original = buffer[0];
            if (original == NewLine || original == (byte)'\r') continue; // držme řádky stejné

            // vyber jiný ASCII znak z naší abecedy, odlišný od původního
            byte replacement;
            int guard = 0;
            do
            {
                replacement = (byte)Alphabet[rnd.Next(Alphabet.Length)];
                if (++guard > 50) replacement = (byte)'X';
            } while (replacement == original || replacement == NewLine);

            // zapiš zpět
            fs.Position = pos;
            buffer[0] = replacement;
            fs.Write(buffer, 0, 1);
            made++;
        }
    }
}
