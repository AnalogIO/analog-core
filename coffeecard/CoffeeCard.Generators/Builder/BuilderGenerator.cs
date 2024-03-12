using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace CoffeeCard.Generators.Builder;

[Generator]
public class BuilderGenerator : IIncrementalGenerator
{
    private const string BuilderForAttribute = "CoffeeCard.Tests.Common.BuilderForAttribute";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<System.Collections.Immutable.ImmutableArray<INamedTypeSymbol>> namedTypeSymbols = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: BuilderForAttribute,
                predicate: IsSyntaxTargetForGeneration,
                transform: GetSemanticTargetForGeneration)
            .Where(t => t is not null).Collect();

        context.RegisterSourceOutput(namedTypeSymbols, (productionContext, array) =>
        {
            foreach (INamedTypeSymbol? typeSymbol in array)
            {
                //Retrieve the entity it is a builder for
                INamedTypeSymbol? entity = (INamedTypeSymbol)typeSymbol.GetAttributes()
                    .Single(attr => attr.AttributeClass.Name == "BuilderForAttribute").ConstructorArguments[0].Value;
                string code = GenerateBuilderCode(typeSymbol, entity);
                SourceText sourceText = SourceText.From(code, Encoding.UTF8);
                productionContext.AddSource($"{typeSymbol.Name}.g.cs", sourceText);
            }
        });
    }

    private static bool IsSyntaxTargetForGeneration(
        SyntaxNode syntaxNode,
        CancellationToken cancellationToken)
    {
        return syntaxNode is ClassDeclarationSyntax;
    }

    private static INamedTypeSymbol GetSemanticTargetForGeneration(GeneratorAttributeSyntaxContext context,
        CancellationToken cancellationToken)
    {
        return (INamedTypeSymbol)context.TargetSymbol;
    }

    private string GenerateBuilderCode(INamedTypeSymbol typeSymbol, ITypeSymbol entity)
    {
        StringBuilder codeBuilder = new StringBuilder();

        _ = codeBuilder.AppendLine("// <auto-generated/>");
        _ = codeBuilder.AppendLine("using System;");
        _ = codeBuilder.AppendLine("using AutoBogus.Conventions;");
        _ = codeBuilder.AppendLine($"using {entity.ContainingNamespace};");
        _ = codeBuilder.AppendLine();
        _ = codeBuilder.AppendLine($"namespace {typeSymbol.ContainingNamespace};");
        _ = codeBuilder.AppendLine();

        _ = codeBuilder.AppendLine($"public partial class {typeSymbol.Name} : BaseBuilder<{entity.Name}>, IBuilder<{typeSymbol.Name}>");
        _ = codeBuilder.AppendLine("{");

        // Retrieve all properties of the given entity
        IEnumerable<IPropertySymbol> properties = entity.GetMembers().OfType<IPropertySymbol>().Where(p => p.Kind == SymbolKind.Property);

        char entityNameChar = entity.Name.ToLowerInvariant()[0];
        // Generate builder methods for all properties
        StringBuilder configBuilder = new StringBuilder();
        foreach (IPropertySymbol? property in properties)
        {
            if (property.Name.Contains("Id"))
            {
                _ = configBuilder.AppendLine($"            .WithSkip<{entity.Name}>(\"{property.Name}\")");
            }
            AddWithPropertyValueToCodeBuilder(codeBuilder: codeBuilder,
                    typeSymbol: typeSymbol,
                    property: property,
                    entityNameChar: entityNameChar);

            AddWithPropertySetterToCodeBuilder(
                codeBuilder: codeBuilder,
                typeSymbol: typeSymbol,
                property: property,
                entityNameChar: entityNameChar);
        }
        AddPrivateConstructorToCodeBuilder(codeBuilder, typeSymbol, configBuilder);

        // End class
        _ = codeBuilder.AppendLine("}");

        return codeBuilder.ToString();
    }

    /// <summary>
    /// Generates a private constructor for the builder, to ensure the simple, or typical methods are used for instantiation
    /// </summary>
    /// <param name="codeBuilder"></param>
    /// <param name="typeSymbol"></param>
    /// <param name="configBuilder"></param>
    private void AddPrivateConstructorToCodeBuilder(StringBuilder codeBuilder, ITypeSymbol typeSymbol, StringBuilder configBuilder)
    {
        _ = codeBuilder.AppendLine(
            $"    private {typeSymbol.Name} ()");
        _ = codeBuilder.AppendLine("    {");
        _ = codeBuilder.AppendLine("        Faker.Configure(builder => builder");
        _ = codeBuilder.Append($"{configBuilder}");
        _ = codeBuilder.AppendLine("            .WithConventions());");
        _ = codeBuilder.AppendLine("    }");
    }

    private void AddWithPropertyValueToCodeBuilder(StringBuilder codeBuilder, ITypeSymbol typeSymbol, IPropertySymbol property, char entityNameChar)
    {
        _ = codeBuilder.AppendLine(
            $"    public {typeSymbol.Name} With{property.Name}({property.Type} {property.Name}Value)");
        _ = codeBuilder.AppendLine("    {");

        _ = codeBuilder.AppendLine(
            $"        Faker.RuleFor({entityNameChar} => {entityNameChar}.{property.Name}, {property.Name}Value);");
        _ = codeBuilder.AppendLine("        return this;");
        _ = codeBuilder.AppendLine("    }");
    }
    private void AddWithPropertySetterToCodeBuilder(StringBuilder codeBuilder, ITypeSymbol typeSymbol, IPropertySymbol property, char entityNameChar)
    {
        _ = codeBuilder.AppendLine(
            $"    public {typeSymbol.Name} With{property.Name}(Func<Bogus.Faker, {property.Type}> {property.Name}Setter)");
        _ = codeBuilder.AppendLine("    {");

        _ = codeBuilder.AppendLine(
            $"        Faker.RuleFor({entityNameChar} => {entityNameChar}.{property.Name}, {property.Name}Setter);");
        _ = codeBuilder.AppendLine("        return this;");
        _ = codeBuilder.AppendLine("    }");
    }
}