using DataSchemaStudio.Application.Common;
using System.Text.RegularExpressions;

namespace DataSchemaStudio.Application.Extensions;

public static class ObjectExtensions
{
    public static string ApplyCasing(this string text, string? casing)
    {
        if (string.IsNullOrWhiteSpace(casing))
            return text;

        return casing.ToLower() switch
        {
            "camelCase" => char.ToLower(text[0]) + text.Substring(1),
            "pascalCase" => char.ToUpper(text[0]) + text.Substring(1),
            "snakeCase" => Regex.Replace(text, "([a-z])([A-Z])", "$1_$2").ToLower(),
            "upperCase" => text.ToUpper(),
            "lowerCase" => text.ToLower(),
            _ => text
        };
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
