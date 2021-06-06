using System;
using System.IO;
using System.Threading.Tasks;
using CTime3.Core.Services.Paths;
using LiteDB;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CTime3.Core.Services.Configurations
{
    public class LiteDBConfigurationService : IConfigurationService, IDisposable
    {
        private readonly ICTimePaths _cTimePaths;
        private readonly ILogger<LiteDBConfigurationService> _logger;
        private readonly LiteDatabase _db;

        public LiteDBConfigurationService(ICTimePaths cTimePaths, ILogger<LiteDBConfigurationService> logger)
        {
            Guard.NotNull(cTimePaths, nameof(cTimePaths));
            Guard.NotNull(logger, nameof(logger));
            
            this._cTimePaths = cTimePaths;
            this._logger = logger;

            var filePath = Path.Combine(this._cTimePaths.DataDirectory, "database.db");
            this._db = new LiteDatabase($"Filename={filePath};Connection=Shared;Upgrade=true");
            
            this.Config = this.ReadExistingConfig();
        }

        public Configuration Config { get; private set; }

        private Configuration ReadExistingConfig()
        {
            var serializedConfig = this._db.GetCollection<SerializedConfiguration>().FindById(SerializedConfiguration.UniqueId);

            if (serializedConfig != null)
            {
                try
                {
                    return JsonConvert.DeserializeObject<Configuration>(serializedConfig.ConfigurationAsJson);
                }
                catch (Exception exception)
                {
                    this._logger.LogError(exception, "An error occurred reading the current configuration. Creating a new one.");
                }
            }

            return new Configuration(null);
        }
        
        public async Task Modify(Func<Configuration, Configuration> changeAction)
        {
            var newConfig = changeAction(this.Config);

            var serializedConfig = new SerializedConfiguration
            {
                Id = SerializedConfiguration.UniqueId,
                ConfigurationAsJson = JsonConvert.SerializeObject(newConfig),
            };
            
            this._db.GetCollection<SerializedConfiguration>().Upsert(serializedConfig);
            this.Config = newConfig;
        }

        public void Dispose()
        {
            this._db.Dispose();
        }

        private class SerializedConfiguration
        {
            public const int UniqueId = 123;
            
            public int Id { get; set; }
            public string ConfigurationAsJson { get; set; }
        }
    }
}