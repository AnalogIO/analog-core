using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CoffeeCard.WebApi.Helpers.Swagger;

/// <summary>
/// Applies the small amount of post-processing needed for Swashbuckle's
/// polymorphic allOf/oneOf output. Discriminator mappings themselves are
/// configured through <c>SelectDiscriminatorValueUsing</c>.
/// </summary>
public sealed class JsonPolymorphicDiscriminatorFilter : IDocumentFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var schemas = swaggerDoc.Components?.Schemas;
        if (schemas is null)
            return;

        var polymorphicBases = schemas
            .Where(pair => pair.Value is OpenApiSchema { Discriminator.Mapping.Count: > 0 })
            .ToDictionary(
                pair => pair.Key,
                pair => (OpenApiSchema)pair.Value,
                StringComparer.OrdinalIgnoreCase
            );

        if (polymorphicBases.Count == 0)
            return;

        // additionalProperties:false cannot be used independently on the base
        // and derived parts of an allOf hierarchy: each part would reject the
        // properties contributed by the other part.
        foreach (var baseSchema in polymorphicBases.Values)
        {
            AllowAdditionalProperties(baseSchema);

            foreach (var derivedReference in baseSchema.Discriminator.Mapping.Values)
            {
                var derivedName = GetReferenceId(derivedReference);
                if (
                    derivedName is null
                    || !schemas.TryGetValue(derivedName, out var derivedSchema)
                    || derivedSchema is not OpenApiSchema concreteDerivedSchema
                )
                {
                    continue;
                }

                AllowAdditionalProperties(concreteDerivedSchema);

                foreach (var allOfSchema in concreteDerivedSchema.AllOf ?? [])
                {
                    if (allOfSchema is OpenApiSchema composedSchema)
                        AllowAdditionalProperties(composedSchema);
                }
            }
        }

        // Swashbuckle puts oneOf on inline property schemas, but puts the
        // discriminator on the base component. Copy the matching discriminator
        // to the inline schema so generators can resolve the union directly.
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
                    AddMatchingDiscriminator(arrayItems, polymorphicBases);
                }
                else if (
                    propertySchema is OpenApiSchema directSchema
                    && directSchema.OneOf is { Count: > 0 }
                )
                {
                    AddMatchingDiscriminator(directSchema, polymorphicBases);
                }
            }
        }
    }

    private static void AddMatchingDiscriminator(
        OpenApiSchema schema,
        IDictionary<string, OpenApiSchema> polymorphicBases
    )
    {
        var baseName = FindMatchingBase(schema, polymorphicBases);
        if (baseName is not null)
            schema.Discriminator = polymorphicBases[baseName].Discriminator;
    }

    private static string FindMatchingBase(
        OpenApiSchema schema,
        IDictionary<string, OpenApiSchema> polymorphicBases
    )
    {
        var oneOfNames = schema
            .OneOf.Select(GetReferenceId)
            .Where(referenceId => referenceId is not null)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var (baseName, baseSchema) in polymorphicBases)
        {
            var derivedNames = baseSchema
                .Discriminator.Mapping.Values.Select(GetReferenceId)
                .Where(referenceId => referenceId is not null)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (derivedNames.SetEquals(oneOfNames))
                return baseName;
        }

        return null;
    }

    private static string GetReferenceId(IOpenApiSchema schema)
    {
        if (schema is OpenApiSchemaReference schemaReference)
            return schemaReference.Reference?.Id ?? schemaReference.Id;

        return null;
    }

    private static void AllowAdditionalProperties(OpenApiSchema schema)
    {
        schema.AdditionalPropertiesAllowed = true;
    }
}
