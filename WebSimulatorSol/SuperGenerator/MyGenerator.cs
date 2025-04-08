using Microsoft.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace WebSimulator
{
    [Generator]
    public class MyGenerator : IIncrementalGenerator
    {
        

        public void Execute(GeneratorExecutionContext context)
        {
            context.AddSource(("myGeneratedFile.cs", SourceText.From(@"
namespace GeneratedNamespace
{
    public class GeneratedClass
    {
        public static void GeneratedMethod()
        {
            Console.WriteLine(""Hello from generated code!"");
        }
    }
}", Encoding.UTF8));
        }

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
