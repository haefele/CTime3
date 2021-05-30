using System;
using System.Threading.Tasks;
using CTime3.Apps.CommandLine.Commands;
using CTime3.Apps.CommandLine.Infrastructure;
using CTime3.Core.Services.Analytics;
using CTime3.Core.Services.Clock;
using CTime3.Core.Services.CTime;
using CTime3.Core.Services.CTime.ImageCache;
using CTime3.Core.Services.CTime.RequestCache;
using CTime3.Core.Services.DataStorage;
using CTime3.Core.Services.GeoLocation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Messaging;
using Spectre.Console.Cli;

namespace CTime3.Apps.CommandLine
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var services = BuildServices();
            var registrar = new TypeRegistrar(services);

            var app = new CommandApp(registrar);
            
            app.Configure(f =>
            {
                f.PropagateExceptions();
                
                f.AddCommand<LoginCommand>("login");
            });

            return await app.RunAsync(args);
        }

        private static ServiceCollection BuildServices()
        {
            var collection = new ServiceCollection();
            
            collection.AddLogging();
            collection.AddSingleton<ICTimeService, CTimeService>();
            collection.AddSingleton<ICTimeRequestCache, CTimeRequestCache>();
            collection.AddSingleton<IClock, RealtimeClock>();
            collection.AddSingleton<IMessenger, WeakReferenceMessenger>();
            collection.AddSingleton<IEmployeeImageCache, EmployeeImageCache>();
            collection.AddSingleton<IGeoLocationService, GeoLocationService>();
            collection.AddSingleton<IAnalyticsService, NullAnalyticsService>();
            collection.AddSingleton<IDataStorage, FileDataStorage>();

            return collection;
        }
    }
}
