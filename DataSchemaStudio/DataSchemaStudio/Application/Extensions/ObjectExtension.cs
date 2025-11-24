using DataSchemaStudio.Application.Common;
using System.Net.NetworkInformation;

namespace DataSchemaStudio.Application.Extensions;

public static class ObjectExtensions
{
    public static string ApplyCasing(this string text, NamingCase? casing)
    {
        if (casing is null)
            return text;

        if (casing.Value.ToString().ToLower() == NamingCase.CamelCase.ToString().ToLower())
            return char.ToLower(text[0]) + text.Substring(1);
        else if (casing.Value.ToString().ToLower() == NamingCase.PascalCase.ToString().ToLower())
            return char.ToLower(text[0]) + text.Substring(1);
        else
            return text;
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
