using DataSchemaStudio.Application.Common;
using DataSchemaStudio.Application.Extensions;
using DataSchemaStudio.Application.Interface;
using DataSchemaStudio.Domain;
using Microsoft.Extensions.Options;
using System.Text;

namespace DataSchemaStudio.Infrastructure.CodeGeneration.Generators.DotNet;

public class DotNetCodeGeneratorService : ICodeGeneratorService
{
    private INamingStrategy _namingDomainStrategy;

    private INamingStrategy _namingDtoStrategy;

    public DotNetCodeGeneratorService(INamingStrategyFactory namingStrategyFactory,
        IOptions<NamingConfig> root)
    {
        _namingDomainStrategy = namingStrategyFactory.Create(root.Value.Domain.Strategy, root.Value.Domain);

        _namingDtoStrategy = namingStrategyFactory.Create(root.Value.Dto.Strategy, root.Value.Dto);
    }
    private void GenerateDomainObjectsRecursive(EntityMetadata entity, StringBuilder sb, HashSet<string> generated)
    {
        if (entity is null)
            return;

        string className = _namingDomainStrategy.ApplyEntityName(entity.EntityName);

        // Ensure normalization is applied before checking
        if (generated.Contains(className))
            return;

        generated.Add(className);

        // Generate all child classes first
        foreach (var child in entity.Relationships)
            GenerateDomainObjectsRecursive(child, sb, generated);

        // Now generate current class
        sb.AppendLine($"public class {className}");

        sb.AppendLine("{");

        // Properties from columns
        foreach (var column in entity.Columns)
        {
            var type = MapToCSharpType(column.DataType, column.IsNullable);

            sb.AppendLine($"    public {type} {column.ColumnName} {{ get; set; }}");
        }

        sb.AppendLine("}");

        sb.AppendLine();
    }

    private void GenerateResponseObjectsRecursive(EntityMetadata entity, StringBuilder sb, HashSet<string> generated)
    {
        string className = _namingDtoStrategy.ApplyEntityName(entity.EntityName);

        // Ensure normalization is applied before checking
        if (generated.Contains(className))
            return;

        generated.Add(className);

        // Generate all child classes first
        foreach (var child in entity.Relationships)
            GenerateResponseObjectsRecursive(child, sb, generated);

        // Now generate current class
        sb.AppendLine($"public class {className}");
        sb.AppendLine("{");

        // Properties from columns
        foreach (var column in entity.Columns)
        {
            var type = MapToCSharpType(column.DataType, column.IsNullable);

            string pPropertyName = _namingDtoStrategy.ApplyPropertyName(column.ColumnName);

            sb.AppendLine($"    public {type} {pPropertyName} {{ get; set; }}");
        }

        // Navigation properties (child lists)
        foreach (var child in entity.Relationships)
        {
            string childClass = _namingDtoStrategy.ApplyEntityName(child.EntityName);

            string childClassPropertyName = _namingDtoStrategy.ApplyPropertyName(child.EntityName);

            if (child.RelationshipType == RelationshipTypeEn.OneToMany)
                sb.AppendLine($"    public List<{childClass}> {childClassPropertyName} {{ get; set; }} = new();");
            else if (child.RelationshipType == RelationshipTypeEn.OneToOne)
                sb.AppendLine($"    public {childClass}? {childClassPropertyName} {{ get; set; }} = new();");
        }

        sb.AppendLine("}");

        sb.AppendLine();
    }

    public string GenerateDomainObjects(EntityMetadata entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        var sb = new StringBuilder();

        var generated = new HashSet<string>();

        GenerateDomainObjectsRecursive(entity, sb, generated);

        return sb.ToString();
    }

    public string GenerateResponseObjects(EntityMetadata entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        var sb = new StringBuilder();

        var generated = new HashSet<string>();

        GenerateResponseObjectsRecursive(entity, sb, generated);

        return sb.ToString();
    }

    public string MapToCSharpType(Type? type, bool isNullable = false)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        string typeName = type switch
        {
            Type t when t == typeof(string) => "string",
            Type t when t == typeof(int) => "int",
            Type t when t == typeof(long) => "long",
            Type t when t == typeof(short) => "short",
            Type t when t == typeof(byte) => "byte",
            Type t when t == typeof(bool) => "bool",
            Type t when t == typeof(decimal) => "decimal",
            Type t when t == typeof(double) => "double",
            Type t when t == typeof(float) => "float",
            Type t when t == typeof(DateTime) => "DateTime",
            Type t when t == typeof(DateOnly) => "DateOnly",
            Type t when t == typeof(Guid) => "Guid",
            _ => type.Name // fallback to the type name
        };

        // Make value types nullable if needed (string is already nullable)
        if (type == typeof(string))
            typeName += "?";
        else if (isNullable && type.IsValueType)
            typeName += "?";

        return typeName;
    }
}


