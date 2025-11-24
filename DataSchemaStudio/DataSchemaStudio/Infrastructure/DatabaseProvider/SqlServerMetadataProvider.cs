using DataSchemaStudio.Application.Common;
using DataSchemaStudio.Application.Interface;
using DataSchemaStudio.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace DataSchemaStudio.Infrastructure.DatabaseProvider;

public class SqlServerMetadataProvider : IMetadataProvider
{
    private readonly string _connectionString;

    public DbProvider DbProviderName => DbProvider.SqlServer;

    public SqlServerMetadataProvider(IOptions<DbOption> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    public async Task<List<ColumnMetaData>> GetColumnMetadataWithSample(string schema, string entity)
    {
        var columns = new List<ColumnMetaData>();

        using var conn = new SqlConnection(_connectionString);

        using var cmd = new SqlCommand($"SELECT TOP 1 * FROM [{schema}].[{entity}]", conn);

        using var adapter = new SqlDataAdapter(cmd);

        await conn.OpenAsync();

        var table = new DataTable();

        adapter.Fill(table);

        DataRow? firstRow = table.Rows.Count > 0 ? table.Rows[0] : null;

        foreach (DataColumn col in table.Columns)
        {
            columns.Add(new ColumnMetaData
            {
                ColumnName = col.ColumnName,
                DataType = col.DataType,
                IsNullable = col.AllowDBNull,
                Ordinal = col.Ordinal,
                SampleValue = firstRow is not null ? firstRow[col] : null
            });
        }

        return columns;
    }
}
