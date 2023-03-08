using CTime3.Core.Services.Storage;

namespace CTime3.Core.Services.Configurations;

public class ConfigurationService : IConfigurationService
{
    private readonly IStorageService _storageService;

    public ConfigurationService(IStorageService storageService)
    {
        Guard.IsNotNull(storageService);

        this._storageService = storageService;

        this.Config = this.ReadExistingConfig();
    }

    public Configuration Config { get; private set; }

    private const string CollectionName = "configuration";
    private const int UniqueId = 123;

    private Configuration ReadExistingConfig()
    {
        // NOTE: Consider refactoring this to use async/await
        var result = this._storageService.GetAsync<Configuration>(CollectionName, UniqueId).Result;
        return result ?? Configuration.GetDefault();
    }

    public async Task Modify(Func<Configuration, Configuration> changeAction)
    {
        var newConfig = changeAction(this.Config);

        await this._storageService.StoreAsync(CollectionName, UniqueId, newConfig);
        this.Config = newConfig;
    }
}
