using DataSchemaStudio.Application.Common;
using DataSchemaStudio.Domain;

namespace DataSchemaStudio.Application.Interface;

public interface IMetadataProvider
{
    DbProvider DbProviderName { get; }
    Task<List<ColumnMetaData>> GetColumnMetadataWithSample(string schema, string table);
}

