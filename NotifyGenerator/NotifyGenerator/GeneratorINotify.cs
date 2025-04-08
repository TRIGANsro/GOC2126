using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace NotifyGenerator
{
    [Generator]
    public class GeneratorINotify : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var memberDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: static (node, _) => node is FieldDeclarationSyntax,
                    transform: static (c, _) => GetMemberWithAttribute(c))
                .Where(member => member != null);

            var collectedMembers = memberDeclarations.Collect();

            // Přidat generování zdrojového kódu
            context.RegisterSourceOutput(collectedMembers, GenerateCode);

        }

        private static MemberWithAttribute GetMemberWithAttribute(GeneratorSyntaxContext context)
        {
            // Získat deklaraci uzlu
            if (context.Node is MemberDeclarationSyntax memberDeclaration)
            {
                // Získat symbol členu
                var model = context.SemanticModel;
                var memberSymbol = model.GetDeclaredSymbol(memberDeclaration);

                // Zkontrolovat, zda má atribut GenerateCode
                var attribute = memberSymbol?.GetAttributes()
                    .FirstOrDefault(attr => attr.AttributeClass?.ToDisplayString() == "MyNamespace.GenerateCodeAttribute");

                if (attribute != null)
                {
                    // Vrátit relevantní data
                    return new MemberWithAttribute(memberSymbol, attribute);
                }
            }
            return null;
        }

        private static void GenerateCode(SourceProductionContext context, ImmutableArray<MemberWithAttribute> members)
        {
            // Generovat zdrojový kód
            var sourceBuilder = new StringBuilder();
            sourceBuilder.AppendLine("namespace Pokusnik");
            sourceBuilder.AppendLine("{");
            sourceBuilder.AppendLine("    public partial class Class1");
            sourceBuilder.AppendLine("    {");

            foreach (var member in members.OfType<MemberWithAttribute>())
            {
                var memberName = member.Symbol.Name;
                var extraInfo = member.Attribute.ConstructorArguments.FirstOrDefault().Value?.ToString();
                sourceBuilder.AppendLine($"        public static void {memberName}Method()");
                sourceBuilder.AppendLine("        {");
                sourceBuilder.AppendLine($"            // Extra info: {extraInfo}");
                sourceBuilder.AppendLine("        }");
            }

            sourceBuilder.AppendLine("    }");
            sourceBuilder.AppendLine("}");

            context.AddSource("Class1.g.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
        }


        private sealed class MemberWithAttribute
        {
            public ISymbol Symbol { get; }
            public AttributeData Attribute { get; }

            internal MemberWithAttribute(ISymbol Symbol, AttributeData Attribute)
            {
                this.Symbol = Symbol;
                this.Attribute = Attribute;
            }
        }
    }
}
