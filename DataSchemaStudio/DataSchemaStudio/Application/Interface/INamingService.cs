using DataSchemaStudio.Application.Common;
using DataSchemaStudio.Domain;

namespace DataSchemaStudio.Application.Interface;

public interface INamingRule
{
    string Apply(string input);
}

public interface INamingStrategy
{
    string ApplyEntityName(string name);
    string ApplyPropertyName(string name);
}

public interface INamingStrategyFactory
{
    INamingStrategy Create(NamingStrategy strategy, NamingLayerConfig section);
}