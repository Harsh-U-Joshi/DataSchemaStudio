namespace DataSchemaStudio.Domain;

public class HttpOperation
{
    public HttpMethod Method { get; set; } = HttpMethod.Get;
    public required string Uri { get; set; }
    public Dictionary<string, string>? Path { get; set; } = new();
    public Dictionary<string, string>? Query { get; set; } = new();
    public Dictionary<string, string>? Body { get; set; } = new();
    public List<int>? Responses { get; set; } = new();
}
