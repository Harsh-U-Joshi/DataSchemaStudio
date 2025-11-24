using DataSchemaStudio.Application.Common;

namespace DataSchemaStudio.Application.Extensions;

public static class ObjectExtensions
{
    public static string ToCamelCase(this string? columnName)
    {
        if (string.IsNullOrWhiteSpace(columnName))
            return "N/A";

        // Special case for EDH prefix
        if (columnName.StartsWith("EDH"))
            columnName = "edh" + columnName.Substring(3);

        // Special case for EDH prefix
        if (columnName.Contains("ID"))
            columnName = columnName.Replace("ID", "Id");

        // Capitalize only the first character
        return char.ToLower(columnName[0]) + columnName.Substring(1);
    }

    public static string ToPascalCase(this string? columnName)
    {
        if (string.IsNullOrWhiteSpace(columnName))
            return "N/A";

        // Special case for EDH prefix
        if (columnName.StartsWith("EDH"))
            columnName = "Edh" + columnName.Substring(3);

        // Special case for EDH prefix
        if (columnName.Contains("ID"))
            columnName = columnName.Replace("ID", "Id");

        // Capitalize only the first character
        return char.ToUpper(columnName[0]) + columnName.Substring(1);
    }

    public static string NormalizeDomainEntityName(this string? entityName)
    {
        string normalizedEntityName = string.Empty;

        if (string.IsNullOrWhiteSpace(entityName))
            return normalizedEntityName;

        if (entityName.StartsWith("vw"))
            normalizedEntityName = entityName.Replace("vw", string.Empty);

        normalizedEntityName = normalizedEntityName + "View";

        return normalizedEntityName;
    }

    public static string NormalizeResponseEntityName(this string? entityName)
    {
        string normalizedEntityName = string.Empty;

        if (string.IsNullOrWhiteSpace(entityName))
            return normalizedEntityName;

        if (entityName.StartsWith("vw"))
            normalizedEntityName = entityName.Replace("vw", string.Empty);

        return normalizedEntityName + "Detail";
    }

    public static string NormalizeResponsePropertyName(this string? propertyName)
    {
        string normalizedEntityName = string.Empty;

        if (string.IsNullOrWhiteSpace(propertyName))
            return normalizedEntityName;

        if (propertyName.StartsWith("vw"))
            normalizedEntityName = propertyName.Replace("vw", string.Empty);

        return normalizedEntityName;
    }

    public static OperationParameterType Parse(this string code)
    {
        return code.ToLower() switch
        {
            "i" => OperationParameterType.Integer,
            "s" => OperationParameterType.String,
            "b" => OperationParameterType.Boolean,
            "d" => OperationParameterType.Date,
            "dt" => OperationParameterType.DateTime,
            _ => OperationParameterType.Unknown
        };
    }
}
