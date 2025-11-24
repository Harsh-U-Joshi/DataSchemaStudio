using DataSchemaStudio.Application.Common;
using DataSchemaStudio.Application.Interface;
using DataSchemaStudio.Domain;
using Microsoft.Extensions.Options;
using NJsonSchema;
using NSwag;
using System.Text.Json.Nodes;

namespace DataSchemaStudio.Infrastructure.Core;

public class OpenApiGeneratorService : IOpenApiGeneratorService
{
    private INamingStrategy _namingStrategy;

    private HttpOperation _httpOperation;

    public OpenApiGeneratorService(
        INamingStrategyFactory namingStrategyFactory,
        IOptions<NamingConfig> namingConfig,
        IOptions<HttpOperation> httpOperation)
    {
        _httpOperation = httpOperation.Value;
        _namingStrategy = namingStrategyFactory.Create(namingConfig.Value.Spec.Strategy, namingConfig.Value.Spec);
    }

    public OpenApiDocument BuildOpenAPISpec(EntityMetadata meta)
    {
        // Create the base OpenAPI document
        var document = new OpenApiDocument
        {
            Info = new OpenApiInfo
            {
                Title = AppConstants.UserInput,
                Version = "1.0.0"
            },
        };

        var pEntity = _namingStrategy.ApplyEntityName(meta.EntityName);

        var visited = new HashSet<string>();

        // Build schema components
        JsonSchema schema = BuildSchema(meta, visited, document);

        // Add schema to Components
        document.Components.Schemas[pEntity] = schema;

        // Build example response
        var exampleResponse = BuildExampleComponent(meta);

        // Build path (GET)
        var pathItem = new OpenApiPathItem();
        var operation = new OpenApiOperation
        {
            Summary = $"Get example {pEntity}",
            OperationId = "get" + pEntity
        };

        // Create the media type
        var mediaType = new OpenApiMediaType
        {
            Schema = new JsonSchema
            {
                Reference = schema
            },
            Example = ConvertJsonNode(exampleResponse)
        };

        // Create the response
        var response = new OpenApiResponse
        {
            Description = "Successful response",
        };

        response.Content.Add("application/json", mediaType);

        operation.Responses["200"] = response;

        // Add GET operation to path
        pathItem.Add("get", operation);

        // Add path to document
        document.Paths["/" + pEntity] = pathItem;

        // Output full OpenAPI JSON
        return document;
    }

    public JsonObject BuildExampleComponent(EntityMetadata entityMeta)
    {
        var json = new JsonObject();

        if (entityMeta == null)
            return json;

        // Add primitive column sample values
        foreach (var col in entityMeta.Columns)
        {
            var propName = _namingStrategy.ApplyPropertyName(col.ColumnName);

            json[propName] = JsonValue.Create(col.SampleValue);
        }

        // No relationships → return base JSON
        if (entityMeta.Relationships == null || entityMeta.Relationships.Count == 0)
            return json;

        foreach (var rel in entityMeta.Relationships)
        {
            var childJson = BuildExampleComponent(rel);

            var entityName = _namingStrategy.ApplyEntityName(rel.EntityName);

            switch (rel.RelationshipType)
            {
                case RelationshipTypeEn.OneToOne:
                    json[entityName] = childJson;
                    break;

                case RelationshipTypeEn.OneToMany:
                    json[entityName] = new JsonArray { childJson };
                    break;
            }
        }

        return json;
    }

    public (JsonObjectType openApiType, string? format) MapToOpenApiDataType(Type? type)
    {
        JsonObjectType openApiType = type switch
        {
            _ when type == typeof(int) || type == typeof(long) => JsonObjectType.Integer,
            _ when type == typeof(float) || type == typeof(double) || type == typeof(decimal) => JsonObjectType.Number,
            _ when type == typeof(bool) => JsonObjectType.Boolean,
            _ => JsonObjectType.String
        };

        string? format = type switch
        {
            _ when type == typeof(int) => "integer",
            _ when type == typeof(float) => "float",
            _ when type == typeof(double) => "double",
            _ when type == typeof(DateTime) => "date-time",
            _ when type == typeof(DateOnly) => "date",
            _ => null
        };

        return (openApiType, format);
    }

    private JsonSchema BuildSchema(EntityMetadata entity, HashSet<string> visited, OpenApiDocument document)
    {
        var pEntity = _namingStrategy.ApplyEntityName(entity.EntityName);

        // Already built → return reference
        if (visited.Contains(pEntity))
            return new JsonSchema
            {
                Description = AppConstants.UserInput,
                Reference = document.Components.Schemas[pEntity]
            };

        visited.Add(pEntity);

        // Create schema
        var schema = new JsonSchema
        {
            Title = pEntity,
            Type = JsonObjectType.Object,
            Description = AppConstants.UserInput
        };

        document.Components.Schemas[pEntity] = schema;

        // Add columns
        foreach (var col in entity.Columns)
        {
            var propName = _namingStrategy.ApplyPropertyName(col.ColumnName);

            var propInfo = MapToOpenApiDataType(col.DataType);

            schema.Properties[propName] = new JsonSchemaProperty
            {
                Type = propInfo.openApiType,
                Format = propInfo.format,
                IsNullableRaw = col.IsNullable,
                Description = propName
            };
        }

        // Add relationships
        foreach (var rel in entity.Relationships)
        {
            var cSchema = BuildSchema(rel, visited, document);

            var cEntity = _namingStrategy.ApplyEntityName(rel.EntityName);

            if (rel.RelationshipType == RelationshipTypeEn.OneToOne)
            {
                schema.Properties[cEntity] = new JsonSchemaProperty
                {
                    Description = AppConstants.UserInput,
                    Reference = cSchema
                };
            }
            else if (rel.RelationshipType == RelationshipTypeEn.OneToMany)
            {
                // Use Reference if the schema is already in Components
                schema.Properties[cEntity] = new JsonSchemaProperty
                {
                    Type = JsonObjectType.Array,
                    Description = AppConstants.UserInput,
                    Item = new JsonSchema { Reference = document.Components.Schemas[cSchema.Title] },
                };
            }
        }

        return schema;
    }

    object ConvertJsonNode(JsonNode node)
    {
        switch (node)
        {
            case JsonObject obj:
                return obj.ToDictionary(kv => kv.Key, kv => ConvertJsonNode(kv.Value!));
            case JsonArray arr:
                return arr.Select(ConvertJsonNode).ToList();
            case JsonValue val:
                return val.GetValue<object>();
            default:
                return null!;
        }
    }
}





