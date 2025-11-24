using DataSchemaStudio.Application.Common;
using DataSchemaStudio.Application.Interface;
using DataSchemaStudio.Domain;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using SQLitePCL;
using System.Data;

namespace DataSchemaStudio.Infrastructure.DatabaseProvider;

public class SqliteMetadataProvider : IMetadataProvider
{
    private readonly string _connectionString;

    public DbProvider DbProviderName => DbProvider.Sqlite;

    public SqliteMetadataProvider(IOptions<DbOption> options)
    {
        _connectionString = options.Value.ConnectionString;
    }

    public async Task<List<ColumnMetaData>> GetColumnMetadataWithSample(string schema, string entity)
    {
        Batteries.Init();

        await Task.Delay(5);

        var result = new List<ColumnMetaData>();

        // SQLite does not support schema -> ignore schema param
        var query = $"SELECT * FROM \"{entity}\" LIMIT 1";

        using var conn = new SqliteConnection(_connectionString);

        using var cmd = new SqliteCommand(query, conn);

        conn.Open();

        using var reader = cmd.ExecuteReader();

        var table = new DataTable();

        table.Load(reader); // Loads schema + data

        DataRow? firstRow = table.Rows.Count > 0 ? table.Rows[0] : null;

        foreach (DataColumn col in table.Columns)
        {
            result.Add(new ColumnMetaData
            {
                ColumnName = col.ColumnName,
                DataType = col.DataType,
                IsNullable = col.AllowDBNull,
                Ordinal = col.Ordinal,
                SampleValue = firstRow is not null ? firstRow[col] : null
            });
        }

        return result;
    }
}
