using DataSchemaStudio.Application.Interface;
using DataSchemaStudio.Domain;
using DataSchemaStudio.Infrastructure.DatabaseProvider;
using Microsoft.Extensions.Options;

namespace DataSchemaStudio.Infrastructure.Core;

public class EntityMetadataService : IEntityMetadataService
{
    private readonly IEnumerable<IMetadataProvider> _providers;

    private readonly DbOption _dbOption;

    public EntityMetadataService(IEnumerable<IMetadataProvider> providers, IOptions<DbOption> dbOption)
    {
        _providers = providers;
        _dbOption = dbOption.Value;
    }

    public async Task<EntityMetadata?> BuildEntityMetadataTreeAsync(AppConfigEntity config)
    {
        var provider = _providers.FirstOrDefault(p => p.DbProviderName.ToString().Equals(_dbOption.DbType.ToString(), StringComparison.OrdinalIgnoreCase));

        if (provider is null)
            throw new InvalidOperationException($"No metadata provider registered for DB type '{_dbOption.DbType}'");

        var pColumns = await provider.GetColumnMetadataWithSample(config.Schema!, config.EntityName!);

        EntityMetadata pEntity = new EntityMetadata
        {
            EntityName = config.EntityName,
            Schema = config.Schema,
            Columns = pColumns,
            RelationshipType = config.RelationshipType,
            Relationships = new()
        };

        // Process relationships
        if (config.Relationship is not null)
        {
            foreach (var relConfig in config.Relationship)
            {
                var relationship = new EntityMetadata
                {
                    Schema = relConfig.Schema,
                    EntityName = relConfig.EntityName,
                    RelationshipType = relConfig.RelationshipType,
                };

                // Recursively load child metadata
                var t1 = await BuildEntityMetadataTreeAsync(relConfig);

                pEntity.Relationships.Add(t1 ?? new());
            }
        }

        return pEntity;
    }
}


