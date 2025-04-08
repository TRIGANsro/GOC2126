using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Text;

namespace MyGenerators;

[Generator]
public class ClassInfoGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: static (node,_) => node is ClassDeclarationSyntax,
                transform: static (ctx,_) => (ClassDeclarationSyntax) ctx.Node 
            ).Where(x => x is not null);

        var compilation = context.CompilationProvider.Combine(provider.Collect());
        
        context.RegisterSourceOutput(compilation, Execute);
    }

    private void Execute(SourceProductionContext context, (Compilation Left, ImmutableArray<ClassDeclarationSyntax> Right) tuple)
    {
        var (compilation, classes) = tuple;

        StringBuilder sb = new();
        bool first = true;
        foreach (var item in classes)
        {
            var symbol = compilation.
                GetSemanticModel(item.SyntaxTree).
                GetDeclaredSymbol(item) as INamedTypeSymbol;

            if (first)
            { 
                sb.Append("    \"" + symbol.ToDisplayString() + "\"" );
                first = false;
            }
            else 
            {
                sb.Append(",\n    \"" + symbol.ToDisplayString() + "\"");
            }
        }

        string code = $$"""
            namespace MyGenerators
            {
                public static class ClassInfo
                {
                    public static string[] Names = 
                    {
                        {{ sb.ToString() }}
                    };
                }
            }
            """;

        context.AddSource("ClassInfo.g.cs", code);
    }
}
