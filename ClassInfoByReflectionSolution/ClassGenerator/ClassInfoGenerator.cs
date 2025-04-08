using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ClassGenerator
{
    [Generator]
    public class ClassInfoGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var provider = context.SyntaxProvider.CreateSyntaxProvider(
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: static (ctx, _) => (ClassDeclarationSyntax) ctx.Node)
                .Where(x => x.Modifiers.Any(m => m.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PublicKeyword)));

            var compilation = context.CompilationProvider.Combine(provider.Collect());

            context.RegisterSourceOutput(compilation, Execute);
        }

        private void Execute(SourceProductionContext context, (Compilation Left, ImmutableArray<ClassDeclarationSyntax> Right) data)
        {
            var (compilation, classes) = data;
            
            List<string> classNames = new();

            foreach (var classDeclaration in classes)
            {
                var symbol = compilation
                    .GetSemanticModel(classDeclaration.SyntaxTree)
                    .GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;

                classNames.Add("\"" + symbol.ToDisplayString() + "\"");
            }

            var names = string.Join(",\n    ", classNames);


            var theCode = $$"""
              namespace ClassInfoGenerated;
                
              public static class ClassInfo
              {
                public static List<string> ClassNames = new()
                {
                    {{ names }}
                };
              }  
              """;

            context.AddSource("ClassInfo.g.cs", theCode);
        }
    }
}
