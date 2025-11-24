using DataSchemaStudio.Domain;

namespace DataSchemaStudio.Application.Interface;

public interface IEntityMetadataService
{
    Task<EntityMetadata?> BuildEntityMetadataTreeAsync(AppConfigEntity config);
}
