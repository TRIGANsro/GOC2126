using System.Reflection;

namespace SourceDemo;

internal class Program
{
    static void Main(string[] args)
    {
        //var data = Assembly.GetEntryAssembly()?.GetTypes()
        //    .Where(t => t.IsClass && t.IsPublic)
        //    .ToArray();


        var data = MyGenerators.ClassInfo.Names;

        if (data is not null)
        {
            foreach (var type in data)
            {
                Console.WriteLine(type);
            }
        }
    }
}
