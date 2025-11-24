using DataSchemaStudio.Application.Interface;
using System.Text.RegularExpressions;

namespace DataSchemaStudio.Infrastructure.Core;

public class RegexNamingRule : INamingRule
{
    private readonly Regex _regex;

    private readonly string _replacement;

    public RegexNamingRule(string pattern, string replacement)
    {
        _regex = new Regex(pattern, RegexOptions.Compiled);

        _replacement = replacement;
    }

    public string Apply(string input)
    {
        return _regex.Replace(input, _replacement);
    }
}


