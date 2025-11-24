using Microsoft.Extensions.DependencyInjection;
using DataSchemaStudio.Application.Interface;
using DataSchemaStudio.Infrastructure.DatabaseProvider;
using Microsoft.Extensions.Configuration;
using DataSchemaStudio.Infrastructure.Core;
using DataSchemaStudio.Infrastructure.CodeGeneration.Generators.DotNet;
using DataSchemaStudio.Domain;
using NJsonSchema;
using Microsoft.Extensions.Options;

public class Program
{
    public static async Task Main(string[] args)
    {
        // 1. Setup DI
        var services = new ServiceCollection();

        ConfigureServices(services);

        var provider = services.BuildServiceProvider();

        // 3. Orchestrate operations
        var rootConfig = provider.GetRequiredService<IOptions<AppConfigEntity>>();
        var codeGenConfig = provider.GetRequiredService<IOptions<CodeGenOption>>();
        var metadataService = provider.GetRequiredService<IEntityMetadataService>();
        var openApiGenerator = provider.GetRequiredService<IOpenApiGeneratorService>();
        var codeGeneratorService = provider.GetRequiredService<ICodeGeneratorService>();

        var entity = rootConfig.Value;

        var codeGenOption = codeGenConfig.Value;

        string specFilePath = Path.Combine(codeGenOption.SpecPath, $"open-api-spec.json");
        string domainFilePath = Path.Combine(codeGenOption.DomainPath, $"domain.cs");
        string dtoFilePath = Path.Combine(codeGenOption.SpecPath, $"dto.cs");

        var meta = await metadataService.BuildEntityMetadataTreeAsync(entity);

        if (meta is null)
            return;

        var openApiSpec = openApiGenerator.BuildOpenAPISpec(meta);

        WriteStringToFile(openApiSpec.ToJson(SchemaType.OpenApi3, new Newtonsoft.Json.Formatting()), specFilePath);

        var classDef = codeGeneratorService.GenerateDomainObjects(meta);

        WriteStringToFile(classDef, domainFilePath);

        var responseDef = codeGeneratorService.GenerateResponseObjects(meta);

        WriteStringToFile(responseDef, dtoFilePath);

        Console.WriteLine("OpenAPI Specs Generated Successfully!");
    }

    public static void WriteStringToFile(string fileContent, string filePath)
    {
        File.WriteAllTextAsync(filePath, fileContent);
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config.json", optional: false)
            .Build();

        // Configuration Bindings
        services.Configure<DbOption>(configuration.GetSection("dbOption"));
        services.Configure<NamingConfig>(configuration.GetSection("naming"));
        services.Configure<HttpOperation>(configuration.GetSection("operation"));
        services.Configure<AppConfigEntity>(configuration.GetSection("data"));
        services.Configure<CodeGenOption>(configuration.GetSection("codeGen"));

        // Naming
        services.AddScoped<INamingStrategyFactory, NamingStrategyFactory>();
        services.AddScoped<INamingStrategy, RegexNamingStrategy>();

        // Database Provider
        services.AddScoped<IMetadataProvider, SqlServerMetadataProvider>();
        services.AddScoped<IMetadataProvider, MySqlMetadataProvider>();
        services.AddScoped<IMetadataProvider, SqliteMetadataProvider>();

        // Core Service
        services.AddScoped<IEntityMetadataService, EntityMetadataService>();
        services.AddScoped<IOpenApiGeneratorService, OpenApiGeneratorService>();
        services.AddScoped<ICodeGeneratorService, DotNetCodeGeneratorService>();
    }
}

