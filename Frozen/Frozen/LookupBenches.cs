using System.Collections.Frozen;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;

namespace Frozen;
[MemoryDiagnoser, Orderer(SummaryOrderPolicy.FastestToSlowest), RankColumn]
public class LookupBenches
{
    [Params(10_000, 100_000)]
    public int N;

    private string[] _keysExisting = default!;
    private string[] _keysMissing = default!;
    private Dictionary<string, int> _dict = default!;
    private FrozenDictionary<string, int> _frozenDict = default!;
    private HashSet<string> _set = default!;
    private FrozenSet<string> _frozenSet = default!;

    private Dictionary<string, int> _dictIgnore = default!;
    private FrozenDictionary<string, int> _frozenDictIgnore = default!;
    private HashSet<string> _setIgnore = default!;
    private FrozenSet<string> _frozenSetIgnore = default!;

    [GlobalSetup]
    public void Setup()
    {
        // testovací data
        _keysExisting = Enumerable.Range(0, N).Select(i => $"user-{i:D7}").ToArray();
        _keysMissing  = Enumerable.Range(N, N).Select(i => $"user-{i:D7}").ToArray();

        _dict = _keysExisting.Select((k, i) => (k, i)).ToDictionary(t => t.k, t => t.i, StringComparer.Ordinal);
        _set  = _keysExisting.ToHashSet(StringComparer.Ordinal);

        _frozenDict = _dict.ToFrozenDictionary();     // default comparer je převzat z _dict
        _frozenSet  = _set.ToFrozenSet();

        // case-insensitive varianta
        _dictIgnore = _keysExisting.Select((k, i) => (k, i))
            .ToDictionary(t => t.k, t => t.i, StringComparer.OrdinalIgnoreCase);
        _setIgnore  = _keysExisting.ToHashSet(StringComparer.OrdinalIgnoreCase);

        _frozenDictIgnore = _dictIgnore.ToFrozenDictionary();
        _frozenSetIgnore  = _setIgnore.ToFrozenSet();
    }

    // --- náklady na vytvoření (build) ---
    [Benchmark(Baseline = true)]
    public Dictionary<string, int> Build_Dictionary()
        => _keysExisting.Select((k, i) => (k, i)).ToDictionary(t => t.k, t => t.i, StringComparer.Ordinal);

    [Benchmark]
    public FrozenDictionary<string, int> Build_FrozenDictionary()
        => _keysExisting 
                .Select((k, i) => (Key: k, Value: i))          // pojmenujeme prvky n-tice
                .ToFrozenDictionary(x => x.Key, x => x.Value,  // selektory
                    StringComparer.Ordinal);
    [Benchmark]
    public HashSet<string> Build_HashSet()
        => _keysExisting.ToHashSet(StringComparer.Ordinal);

    [Benchmark]
    public FrozenSet<string> Build_FrozenSet()
        => _keysExisting.ToFrozenSet(StringComparer.Ordinal);

    // --- lookup: existující klíče ---
    [Benchmark]
    public int Dict_TryGetValue_Hit()
    {
        int sum = 0;
        foreach (var k in _keysExisting)
            if (_dict.TryGetValue(k, out var v)) sum += v;
        return sum;
    }

    [Benchmark]
    public int FrozenDict_TryGetValue_Hit()
    {
        int sum = 0;
        foreach (var k in _keysExisting)
            if (_frozenDict.TryGetValue(k, out var v)) sum += v;
        return sum;
    }

    [Benchmark]
    public int Set_Contains_Hit()
    {
        int sum = 0;
        foreach (var k in _keysExisting)
            if (_set.Contains(k)) sum++;
        return sum;
    }

    [Benchmark]
    public int FrozenSet_Contains_Hit()
    {
        int sum = 0;
        foreach (var k in _keysExisting)
            if (_frozenSet.Contains(k)) sum++;
        return sum;
    }

    // --- lookup: chybějící klíče (miss) ---
    [Benchmark]
    public int Dict_TryGetValue_Miss()
    {
        int sum = 0;
        foreach (var k in _keysMissing)
            if (_dict.TryGetValue(k, out var v)) sum += v;
        return sum;
    }

    [Benchmark]
    public int FrozenDict_TryGetValue_Miss()
    {
        int sum = 0;
        foreach (var k in _keysMissing)
            if (_frozenDict.TryGetValue(k, out var v)) sum += v;
        return sum;
    }

    [Benchmark]
    public int Set_Contains_Miss()
    {
        int sum = 0;
        foreach (var k in _keysMissing)
            if (_set.Contains(k)) sum++;
        return sum;
    }

    [Benchmark]
    public int FrozenSet_Contains_Miss()
    {
        int sum = 0;
        foreach (var k in _keysMissing)
            if (_frozenSet.Contains(k)) sum++;
        return sum;
    }

    // --- case-insensitive ---
    [Benchmark]
    public int FrozenDict_IgnoreCase_Hit()
    {
        int sum = 0;
        foreach (var k in _keysExisting)
            if (_frozenDictIgnore.TryGetValue(k.ToUpperInvariant(), out var v)) sum += v;
        return sum;
    }

    [Benchmark]
    public int Dict_IgnoreCase_Hit()
    {
        int sum = 0;
        foreach (var k in _keysExisting)
            if (_dictIgnore.TryGetValue(k.ToUpperInvariant(), out var v)) sum += v;
        return sum;
    }
}