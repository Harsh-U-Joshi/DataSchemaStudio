using DataSchemaStudio.Domain;

namespace DataSchemaStudio.Application.Interface;

public interface ICodeGeneratorService
{
    string GenerateDomainObjects(EntityMetadata entity);
    string GenerateResponseObjects(EntityMetadata entity);
}
