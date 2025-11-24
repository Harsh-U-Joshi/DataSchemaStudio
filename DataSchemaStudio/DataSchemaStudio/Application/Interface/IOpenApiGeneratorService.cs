using DataSchemaStudio.Domain;
using NJsonSchema;
using NSwag;
using System.Text.Json.Nodes;

namespace DataSchemaStudio.Application.Interface;

public interface IOpenApiGeneratorService
{
    OpenApiDocument BuildOpenAPISpec(EntityMetadata meta);
    JsonObject BuildExampleComponent(EntityMetadata entity);
    (JsonObjectType openApiType, string? format) MapToOpenApiDataType(Type? type);
}
