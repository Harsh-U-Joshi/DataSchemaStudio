using DataSchemaStudio.Application.Interface;

namespace DataSchemaStudio.Infrastructure.Naming;

public class IdentityNamingStrategy : INamingStrategy
{
    public string ApplyEntityName(string name) => name;
    public string ApplyPropertyName(string name) => name;
}

