namespace DataSchemaStudio.Domain;

public class CodeGenOption
{
    public string SpecPath { get; set; } = "";
    public string DomainPath { get; set; } = "";
    public string DtoPath { get; set; } = "";

    // Optional
    public string Template { get; set; } = "default";
}

