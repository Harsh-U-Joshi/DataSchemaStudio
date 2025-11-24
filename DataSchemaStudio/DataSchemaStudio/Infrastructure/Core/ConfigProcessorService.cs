using DataSchemaStudio.Application.Convertors;
using DataSchemaStudio.Application.Interface;
using DataSchemaStudio.Domain;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DataSchemaStudio.Infrastructure.Core;

public class ConfigProcessorService : IConfigProcessorService
{
    public async Task<RootConfig?> LoadConfig(string path)
    {
        var json = await File.ReadAllTextAsync(path);

        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        options.Converters.Add(new HttpMethodConverter());
        options.Converters.Add(new JsonStringEnumConverter());

        return JsonSerializer.Deserialize<RootConfig>(json, options);
    }
}

