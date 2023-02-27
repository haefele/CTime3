﻿using CommunityToolkit.Mvvm.Messaging;
using CTime3.Apps.CommandLine.Commands;
using CTime3.Apps.CommandLine.Infrastructure;
using CTime3.Core.Services.Analytics;
using CTime3.Core.Services.ApplicationEnvironment;
using CTime3.Core.Services.Clock;
using CTime3.Core.Services.Configurations;
using CTime3.Core.Services.CTime;
using CTime3.Core.Services.CTime.ImageCache;
using CTime3.Core.Services.CTime.RequestCache;
using CTime3.Core.Services.Statistics;
using CTime3.Core.Services.Storage;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace CTime3.Apps.CommandLine;

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

            f.AddCommand<LoginCommand>("login")
                .WithDescription("Login so you can use the other commands.");

            f.AddCommand<LogoutCommand>("logout")
                .WithDescription("Logout the current user.");

            f.AddCommand<ListCommand>("list")
                .WithDescription("Shows a list of your previous times.");

            f.AddCommand<StatusCommand>("status")
                .WithDescription("Shows whether you're currently checked-in or not, and your current time.");
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
        collection.AddSingleton<IAnalyticsService, NullAnalyticsService>();
        collection.AddSingleton<IConfigurationService, ConfigurationService>();
        collection.AddSingleton<IStorageService, LiteDBStorageService>();
        collection.AddSingleton<IStatisticsService, StatisticsService>();
        collection.AddSingleton<IApplicationEnvironment>(new ApplicationEnvironment("haefele", "c-Time CLI"));

        return collection;
    }
}
