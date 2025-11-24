using DataSchemaStudio.Application.Common;

namespace DataSchemaStudio.Domain;

public class NamingConfig
{
    public NamingLayerConfig? Domain { get; set; }
    public NamingLayerConfig? Dto { get; set; }
    public NamingLayerConfig? Spec { get; set; }
}

public class NamingLayerConfig
{
    public NamingStrategy Strategy { get; set; } = NamingStrategy.None;
    public string? DefaultEntityCasing { get; set; }
    public string? DefaultPropertyCasing { get; set; }
    public List<NamingRules>? EntityRules { get; set; }
    public List<NamingRules>? PropertyRules { get; set; }
}

public class NamingRules
{
    public string? Match { get; set; }
    public string? Replace { get; set; }
}

