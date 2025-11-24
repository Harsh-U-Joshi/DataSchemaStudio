using DataSchemaStudio.Application.Common;

namespace DataSchemaStudio.Domain;

public class DbOption
{
    public DbProvider DbType { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
}

public class AuthOptions
{
    public DbAuthType Type { get; set; } = DbAuthType.None;
    public AzureAdOptions? Options { get; set; }
}

public class AzureAdOptions
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? TenantId { get; set; }
}
