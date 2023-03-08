namespace CTime3.Core.Services.Storage;

public interface IStorageService
{
    Task StoreAsync<T>(string collection, int id, T value);
    Task<T?> GetAsync<T>(string collection, int id);
}
