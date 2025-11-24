using DataSchemaStudio.Domain;

namespace DataSchemaStudio.Application.Interface;

public interface IConfigProcessorService
{
    Task<RootConfig?> LoadConfig(string filePath);
}
