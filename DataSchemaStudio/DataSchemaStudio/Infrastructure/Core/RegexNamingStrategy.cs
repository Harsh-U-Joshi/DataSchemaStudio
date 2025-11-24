using DataSchemaStudio.Application.Interface;

namespace DataSchemaStudio.Infrastructure.Core;

public class RegexNamingStrategy : INamingStrategy
{
    private readonly List<INamingRule> _entityRules;

    private readonly List<INamingRule> _propertyRules;

    public RegexNamingStrategy(
        IEnumerable<INamingRule> entityRules,
        IEnumerable<INamingRule> propertyRules)
    {
        _entityRules = entityRules.ToList();
        _propertyRules = propertyRules.ToList();
    }

    public string ApplyEntityName(string name)
    {
        foreach (var rule in _entityRules)
            name = rule.Apply(name);

        name = char.ToLower(name[0]) + name.Substring(1);

        return name;
    }

    public string ApplyPropertyName(string name)
    {
        foreach (var rule in _propertyRules)
            name = rule.Apply(name);

        name = char.ToLower(name[0]) + name.Substring(1);

        return name;
    }
}

