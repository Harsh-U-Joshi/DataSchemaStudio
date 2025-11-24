using DataSchemaStudio.Domain;

namespace DataSchemaStudio.Application.Interface;

public interface IHttpOperationLoader
{
    Task<HttpOperation> LoadAsync(string filePath);
}