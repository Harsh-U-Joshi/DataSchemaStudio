using DataSchemaStudio.Application.Common;
using DataSchemaStudio.Application.Extensions;
using DataSchemaStudio.Application.Interface;

namespace DataSchemaStudio.Infrastructure.Core;

public class RegexNamingStrategy : INamingStrategy
{
    private readonly List<INamingRule> _entityRules;

    private readonly List<INamingRule> _propertyRules;

    private readonly NamingCase? _defaultEntityCasing;

    private readonly NamingCase? _defaultPropertyCasing;

    public RegexNamingStrategy(
        IEnumerable<INamingRule> entityRules,
        IEnumerable<INamingRule> propertyRules,
        NamingCase? defaultEntityCasing,
        NamingCase? defaultPropertyCasing)
    {
        _entityRules = entityRules.ToList();
        _propertyRules = propertyRules.ToList();
        _defaultEntityCasing = defaultEntityCasing;
        _defaultPropertyCasing = defaultPropertyCasing;
    }

    public string ApplyEntityName(string name)
    {
        foreach (var rule in _entityRules)
            name = rule.Apply(name);

        return name.ApplyCasing(_defaultEntityCasing);
    }

    public string ApplyPropertyName(string name)
    {
        foreach (var rule in _propertyRules)
            name = rule.Apply(name);

        return name.ApplyCasing(_defaultPropertyCasing);
    }
}

