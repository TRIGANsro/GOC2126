using System.Buffers;

var pocet = 1_000_000;

for (int i = 0; i < pocet; i++)
{
    AkceAlfa(1000);
}

PrintGCInfo();

for (int i = 0; i < pocet; i++)
{
    AkceAlfa(10000);
}

PrintGCInfo();

for (int i = 0; i < pocet; i++)
{
    AkceBeta();
}

PrintGCInfo();

void AkceAlfa(int pocet)
{
    int[] pole = new int[pocet];
    for (int i = 0; i < 100; i++)
    {
        pole[i] = i;

    }

    var sum = pole.Sum();
}

void AkceBeta()
{
    int[] pole = ArrayPool<int>.Shared.Rent(30000);
    for (int i = 0; i < 100; i++)
    {
        pole[i] = i;

    }

    //var sum = pole.Sum();
    ArrayPool<int>.Shared.Return(pole);
}


void PrintGCInfo()
{
    var info = GC.GetGCMemoryInfo();

    Console.WriteLine("Total memory: {0} bytes", info.TotalAvailableMemoryBytes);
    Console.WriteLine("High memory load threshold: {0} bytes", info.HighMemoryLoadThresholdBytes);
    Console.WriteLine("Memory load: {0} bytes", info.MemoryLoadBytes);
    Console.WriteLine("Fragmentation: {0} bytes", info.FragmentedBytes);
    Console.WriteLine("Heap Size Bytes: {0}", info.HeapSizeBytes);
    Console.WriteLine("Total committed memory: {0} bytes", info.TotalCommittedBytes);
    Console.WriteLine("Compacted: {0} bytes", info.Compacted);
    Console.WriteLine("Pause time: {0} %", info.PauseTimePercentage);
    Console.WriteLine("Pause duration: {0} ms", info.PauseDurations[0].Milliseconds);
    Console.WriteLine("Generation 0 info before: {0}", info.GenerationInfo[0].SizeBeforeBytes);
    Console.WriteLine("Generation 0 info after: {0}", info.GenerationInfo[0].SizeAfterBytes);
    Console.WriteLine("Generation 1 info before: {0}", info.GenerationInfo[1].SizeBeforeBytes);
    Console.WriteLine("Generation 1 info after: {0}", info.GenerationInfo[1].SizeAfterBytes);
    Console.WriteLine("Generation 2 info before: {0}", info.GenerationInfo[2].SizeBeforeBytes);
    Console.WriteLine("Generation 2 info after: {0}", info.GenerationInfo[2].SizeAfterBytes);


    Console.WriteLine("Generation 0: {0} x", GC.CollectionCount(0));
    Console.WriteLine("Generation 1: {0} x", GC.CollectionCount(1));
    Console.WriteLine("Generation 2: {0} x", GC.CollectionCount(2));
}