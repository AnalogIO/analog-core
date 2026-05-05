using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CoffeeCard.WebApi.Helpers.Swagger;

/// <summary>
/// Enriches OpenAPI discriminators with value→schema mappings derived from
/// <see cref="JsonDerivedTypeAttribute"/> annotations, and replaces inline
/// <c>oneOf</c> references to derived types with a <c>$ref</c> to the
/// discriminated base type so NSwag generates correct polymorphic clients.
/// </summary>
public class JsonPolymorphicDiscriminatorFilter : IDocumentFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var schemas = swaggerDoc.Components?.Schemas;
        if (schemas is null)
            return;

        // Build lookup from schema name → schema name (case-insensitive)
        var schemaNameLookup = schemas.Keys.ToDictionary(
            k => k,
            k => k,
            StringComparer.OrdinalIgnoreCase
        );

        // Collect schemas that have a discriminator and add mappings
        var discriminatedBases = new Dictionary<string, HashSet<string>>();

        foreach (var (schemaName, schema) in schemas)
        {
            if (schema is not OpenApiSchema concreteSchema)
                continue;

            if (concreteSchema.Discriminator is null)
                continue;

            var clrType = AppDomain
                .CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try
                    {
                        return a.GetTypes();
                    }
                    catch
                    {
                        return Array.Empty<Type>();
                    }
                })
                .FirstOrDefault(t =>
                    string.Equals(t.Name, schemaName, StringComparison.OrdinalIgnoreCase)
                    && t.GetCustomAttributes(typeof(JsonPolymorphicAttribute), false).Length > 0
                );

            if (clrType is null)
                continue;

            var derivedTypes = clrType
                .GetCustomAttributes(typeof(JsonDerivedTypeAttribute), inherit: false)
                .Cast<JsonDerivedTypeAttribute>()
                .ToList();

            if (derivedTypes.Count == 0)
                continue;

            concreteSchema.Discriminator.Mapping ??=
                new Dictionary<string, OpenApiSchemaReference>();

            var derivedSchemaNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var derived in derivedTypes)
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
                    concreteSchema.Discriminator.Mapping[discriminatorValue] =
                        new OpenApiSchemaReference(derivedSchemaName);
                    derivedSchemaNames.Add(derivedSchemaName);
                }
            }

            discriminatedBases[schemaName] = derivedSchemaNames;
        }

        // Replace oneOf references to all derived types with a $ref to the base type
        if (discriminatedBases.Count == 0)
            return;

        foreach (var (_, schema) in schemas)
        {
            if (schema is not OpenApiSchema s || s.Properties is null)
                continue;

            var propsToUpdate = new List<(string key, IOpenApiSchema replacement)>();

            foreach (var (propName, propSchema) in s.Properties)
            {
                switch (propSchema)
                {
                    // Array property: check items.oneOf
                    case OpenApiSchema arraySchema
                        when arraySchema.Items is OpenApiSchema itemsSchema
                            && itemsSchema.OneOf is { Count: > 0 }:
                    {
                        var baseName = FindMatchingBase(itemsSchema.OneOf, discriminatedBases);
                        if (baseName is not null)
                        {
                            arraySchema.Items = new OpenApiSchemaReference(baseName);
                        }

                        break;
                    }
                    // Direct property: check oneOf
                    case OpenApiSchema directSchema when directSchema.OneOf is { Count: > 0 }:
                    {
                        var baseName = FindMatchingBase(directSchema.OneOf, discriminatedBases);
                        if (baseName is not null)
                        {
                            propsToUpdate.Add((propName, new OpenApiSchemaReference(baseName)));
                        }

                        break;
                    }
                }
            }

            foreach (var (key, replacement) in propsToUpdate)
            {
                s.Properties[key] = replacement;
            }
        }
    }

    private static string? FindMatchingBase(
        IList<IOpenApiSchema> oneOfEntries,
        Dictionary<string, HashSet<string>> discriminatedBases
    )
    {
        var oneOfRefs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in oneOfEntries)
        {
            if (entry is OpenApiSchemaReference schemaRef)
            {
                // In Microsoft.OpenApi v2, the schema name is on Reference.Id, not directly on the schema ref
                var refId = schemaRef.Reference?.Id ?? schemaRef.Id;
                if (refId is not null)
                    oneOfRefs.Add(refId);
            }
        }

        foreach (var (baseName, derivedNames) in discriminatedBases)
        {
            if (derivedNames.SetEquals(oneOfRefs))
                return baseName;
        }

        return null;
    }
}
