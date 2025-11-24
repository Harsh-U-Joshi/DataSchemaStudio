using DataSchemaStudio.Application.Common;

namespace DataSchemaStudio.Domain;

public class RootConfig
{
    public DbOption? DbOption { get; set; }
    public NamingConfig? Naming { get; set; }
    public HttpOperation? Operation { get; set; }
    public AppConfigEntity? Data { get; set; }
}

public class AppConfigEntity
{
    public string? Schema { get; set; }
    public string? EntityName { get; set; } = null!;
    public RelationshipTypeEn? RelationshipType { get; set; }
    public List<AppConfigEntity>? Relationship { get; set; }
}


