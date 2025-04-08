using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;

public class TestExpression
{
    private Person[] persons = Person.GetPeople(10_000);

    [Benchmark]
    public void Standard()
    {
        var data = persons.Where( person => person.Age == 23).ToArray();
    }

    private static readonly Expression<Func<Person, bool>> predicate = person => person.Age == 23;

    [Benchmark]
    public void Prepared()
    {
        var data = persons.AsQueryable().Where(predicate).ToArray();
    }

    private static readonly Func<Person, bool> compiled = predicate.Compile();

    [Benchmark]
    public void Compiled()
    {
        var data = persons.Where(compiled).ToArray();
    }
}