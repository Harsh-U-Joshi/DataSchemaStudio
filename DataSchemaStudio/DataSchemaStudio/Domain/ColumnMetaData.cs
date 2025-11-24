namespace DataSchemaStudio.Domain;
public class ColumnMetaData
{
    public string? ColumnName { get; set; }
    public Type? DataType { get; set; }
    public bool IsNullable { get; set; }
    public int Ordinal { get; set; }
    public object? SampleValue { get; set; }
}