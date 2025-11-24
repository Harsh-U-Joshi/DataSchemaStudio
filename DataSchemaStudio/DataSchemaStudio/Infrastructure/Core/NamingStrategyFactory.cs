using DataSchemaStudio.Application.Common;
using DataSchemaStudio.Application.Interface;
using DataSchemaStudio.Domain;
using DataSchemaStudio.Infrastructure.Naming;

namespace DataSchemaStudio.Infrastructure.Core;

public class NamingStrategyFactory : INamingStrategyFactory
{
    public INamingStrategy Create(NamingStrategy strategy, NamingLayerConfig section)
    {
        if (strategy == NamingStrategy.None)
            return new IdentityNamingStrategy();

        if (strategy == NamingStrategy.Regex)
        {
            var entityRules = section.EntityRules?
                .Select(r => new RegexNamingRule(r.Match, r.Replace))
                ?? Enumerable.Empty<INamingRule>();

            var propertyRules = section.PropertyRules?
                .Select(r => new RegexNamingRule(r.Match, r.Replace))
                ?? Enumerable.Empty<INamingRule>();

            return new RegexNamingStrategy(entityRules, propertyRules, section.DefaultEntityCasing, section.DefaultPropertyCasing);
        }

        // fallback
        return new IdentityNamingStrategy();
    }
}
