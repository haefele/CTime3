using CTime3.Core.Services.ApplicationEnvironment;
using LiteDB;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CTime3.Core.Services.Storage;

public class LiteDBStorageService : IStorageService, IDisposable
{
    private readonly LiteDatabase _db;

    public LiteDBStorageService(IOptions<CTimeApplicationOptions> ctimeApplicationOptions)
    {
        Guard.IsNotNull(ctimeApplicationOptions);

        // NOTE: Consider checking ctimeApplicationOptions.Value.CompanyName and ctimeApplicationOptions.Value.AppName for valid file path characters
        var directoryPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            ctimeApplicationOptions.Value.CompanyName,
            ctimeApplicationOptions.Value.AppName);

        Directory.CreateDirectory(directoryPath);

        var filePath = Path.Combine(directoryPath, "database.db");

        this._db = new LiteDatabase($"Filename={filePath};Connection=Shared;Upgrade=true");
    }

    public Task StoreAsync<T>(string collection, int id, T value)
    {
        Guard.IsNotNullOrWhiteSpace(collection);
        Guard.IsNotDefault(id);
        Guard.IsNotNull(value);

        var serialized = new SerializedData
        {
            Id = id,
            DataAsJson = JsonConvert.SerializeObject(value),
        };

        this._db.GetCollection<SerializedData>(collection).Upsert(id, serialized);
        return Task.CompletedTask;
    }

    public Task<T?> GetAsync<T>(string collection, int id)
    {
        Guard.IsNotNullOrWhiteSpace(collection);
        Guard.IsNotDefault(id);

        var serialized = this._db.GetCollection<SerializedData>(collection).FindById(id);
        return serialized is { DataAsJson: not null }
            ? Task.FromResult(JsonConvert.DeserializeObject<T>(serialized.DataAsJson))
            : Task.FromResult<T?>(default);
    }

    public void Dispose()
    {
        this._db.Dispose();
    }

    private class SerializedData
    {
        public required int Id { get; init; }
        public required string DataAsJson { get; init; }
    }
}
