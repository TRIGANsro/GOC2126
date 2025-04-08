using System.Reflection;
using static System.Console;

namespace ClassInfoGenerated;

internal class Program
{
    static void Main(string[] args)
    {
        //var entryAssembly = Assembly.GetEntryAssembly();
        //if (entryAssembly == null)
        //{
        //    WriteLine("Entry assembly is null.");
        //    return;
        //}

        //List<Type> classTypes = entryAssembly
        //    .GetTypes()
        //    .Where(x => x.IsPublic && x.IsClass && (x.BaseType != typeof(Attribute)))
        //    .ToList();

        foreach (var className in ClassInfo.ClassNames)
        {
            WriteLine("Class: " + className);

            //var properties = classType.GetProperties()
            //    .Select(x => new { Attribute = x.GetCustomAttribute<MyInfoAttribute>(), Property = x })
            //    .Where(x => x.Attribute != null)
            //    .ToList();

            //foreach (var x in properties)
            //{
            //    WriteLine("Property: " + x.Property.Name);
            //    WriteLine("ExtraInfo: " + x.Attribute?.ExtraInfo);
            //}
        }
    }
}
