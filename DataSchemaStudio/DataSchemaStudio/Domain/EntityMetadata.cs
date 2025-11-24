using DataSchemaStudio.Application.Common;

namespace DataSchemaStudio.Domain;

public class Root
{
    public NamingConfig? NamingConfig { get; set; }
    public EntityMetadata? Entity { get; set; }
}

public class EntityMetadata
{
    public string? Schema { get; set; }
    public string? EntityName { get; set; }
    public List<ColumnMetaData> Columns { get; set; } = new();
    public RelationshipTypeEn? RelationshipType { get; set; }
    public List<EntityMetadata> Relationships { get; set; } = new();
}
