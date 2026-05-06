MaybePrint(42.0);

static void MaybePrint<T>(T value)
{
    if (value is int)
        Console.WriteLine(value);
}