using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CoffeeCard.WebApi.Helpers.Swagger;

/// <summary>
/// Adds discriminator mappings from <see cref="JsonDerivedTypeAttribute"/> and
/// keeps the generated polymorphic inheritance schemas composable.
/// </summary>
public class JsonPolymorphicDiscriminatorFilter : IDocumentFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var schemas = swaggerDoc.Components?.Schemas;
        if (schemas is null)
            return;

        var schemaNameLookup = schemas.Keys.ToDictionary(
            key => key,
            key => key,
            StringComparer.OrdinalIgnoreCase
        );

        var discriminatedBases = new Dictionary<string, HashSet<string>>();
        var discriminators = new Dictionary<string, OpenApiDiscriminator>(
            StringComparer.OrdinalIgnoreCase
        );

        foreach (var (schemaName, schema) in schemas)
        {
            if (schema is not OpenApiSchema concreteSchema)
                continue;

            if (concreteSchema.Discriminator is null)
                continue;

            var clrType = FindPolymorphicType(schemaName);
            if (clrType is null)
                continue;

            var derivedSchemaNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (
                var derived in clrType
                    .GetCustomAttributes(typeof(JsonDerivedTypeAttribute), inherit: false)
                    .Cast<JsonDerivedTypeAttribute>()
            )
            {
                if (derived.TypeDiscriminator is not string discriminatorValue)
                    continue;

                if (
                    schemaNameLookup.TryGetValue(
                        derived.DerivedType.Name,
                        out var derivedSchemaName
                    )
                )
                {
                    concreteSchema.Discriminator.Mapping ??=
                        new Dictionary<string, OpenApiSchemaReference>();
                    concreteSchema.Discriminator.Mapping[discriminatorValue] =
                        new OpenApiSchemaReference(derivedSchemaName);
                    derivedSchemaNames.Add(derivedSchemaName);
                }
            }

            if (derivedSchemaNames.Count > 0)
            {
                discriminatedBases[schemaName] = derivedSchemaNames;
                discriminators[schemaName] = concreteSchema.Discriminator;
            }
        }

        // Swashbuckle emits additionalProperties:false for object schemas. That
        // is not composable with allOf: the base rejects derived properties and
        // the derived inline schema rejects base properties. The JSON serializer
        // accepts the combined object, so make every component in this
        // polymorphic hierarchy open.
        foreach (var (schemaName, schema) in schemas)
        {
            if (schema is not OpenApiSchema concreteSchema)
                continue;

            var isDiscriminatedBase = discriminatedBases.ContainsKey(schemaName);
            var isDerivedSchema = discriminatedBases.Values.Any(
                derivedNames => derivedNames.Contains(schemaName)
            );

            if (!isDiscriminatedBase && !isDerivedSchema)
                continue;

            AllowAdditionalProperties(concreteSchema);

            foreach (var allOfSchema in concreteSchema.AllOf ?? [])
            {
                if (allOfSchema is OpenApiSchema composedSchema)
                    AllowAdditionalProperties(composedSchema);
            }
        }

        // The oneOf produced for a property is an inline schema. Put the
        // discriminator on that schema as well as on the base component. This
        // is understood by generators such as NSwag and still leaves the
        // standard oneOf intact for other generators.
        foreach (var schema in schemas.Values.OfType<OpenApiSchema>())
        {
            if (schema.Properties is null)
                continue;

            foreach (var propertySchema in schema.Properties.Values)
            {
                if (
                    propertySchema is OpenApiSchema arraySchema
                    && arraySchema.Items is OpenApiSchema arrayItems
                    && arrayItems.OneOf is { Count: > 0 }
                )
                {
                    AddInlineDiscriminator(arrayItems, discriminatedBases, discriminators);
                }
                else if (
                    propertySchema is OpenApiSchema directSchema
                    && directSchema.OneOf is { Count: > 0 }
                )
                {
                    AddInlineDiscriminator(directSchema, discriminatedBases, discriminators);
                }
            }
        }
    }

    private static void AddInlineDiscriminator(
        OpenApiSchema schema,
        Dictionary<string, HashSet<string>> discriminatedBases,
        Dictionary<string, OpenApiDiscriminator> discriminators
    )
    {
        var baseName = FindMatchingBase(schema.OneOf, discriminatedBases);
        if (baseName is null)
            return;

        if (discriminators.TryGetValue(baseName, out var discriminator))
            schema.Discriminator = discriminator;
    }

    private static string FindMatchingBase(
        IList<IOpenApiSchema> oneOfEntries,
        Dictionary<string, HashSet<string>> discriminatedBases
    )
    {
        var oneOfRefs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in oneOfEntries)
        {
            if (entry is not OpenApiSchemaReference schemaRef)
                continue;

            var refId = schemaRef.Reference?.Id ?? schemaRef.Id;
            if (refId is not null)
                oneOfRefs.Add(refId);
        }

        foreach (var (baseName, derivedNames) in discriminatedBases)
        {
            if (derivedNames.SetEquals(oneOfRefs))
                return baseName;
        }

        return null;
    }

    private static Type FindPolymorphicType(string schemaName)
    {
        return AppDomain
            .CurrentDomain.GetAssemblies()
            .SelectMany(assembly =>
            {
                try
                {
                    return assembly.GetTypes();
                }
                catch
                {
                    return Array.Empty<Type>();
                }
            })
            .FirstOrDefault(type =>
                string.Equals(type.Name, schemaName, StringComparison.OrdinalIgnoreCase)
                && type.GetCustomAttributes(typeof(JsonPolymorphicAttribute), inherit: false).Length > 0
            );
    }

    private static void AllowAdditionalProperties(OpenApiSchema schema)
    {
        schema.AdditionalPropertiesAllowed = true;
    }
}
