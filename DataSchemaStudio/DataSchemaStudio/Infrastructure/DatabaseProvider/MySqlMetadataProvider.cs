using DataSchemaStudio.Application.Common;
using DataSchemaStudio.Application.Interface;
using DataSchemaStudio.Domain;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System.Data;

namespace DataSchemaStudio.Infrastructure.DatabaseProvider;

public class MySqlMetadataProvider : IMetadataProvider
{
    private readonly string _connectionString;

    public DbProvider DbProviderName => DbProvider.MySQL;

    public MySqlMetadataProvider(IOptions<DbOption> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    public async Task<List<ColumnMetaData>> GetColumnMetadataWithSample(string schema, string entity)
    {
        var columns = new List<ColumnMetaData>();

        using var conn = new MySqlConnection(_connectionString);

        string query = $"SELECT * FROM `{schema}`.`{entity}` LIMIT 1";

        await conn.OpenAsync();

        using var cmd = new MySqlCommand(query, conn);

        using var adapter = new MySqlDataAdapter(cmd);

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
