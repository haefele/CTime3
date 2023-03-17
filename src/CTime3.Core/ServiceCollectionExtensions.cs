using CommunityToolkit.Mvvm.Messaging;
using CTime3.Core.Services.Alerts;
using CTime3.Core.Services.Analytics;
using CTime3.Core.Services.Clock;
using CTime3.Core.Services.Configurations;
using CTime3.Core.Services.CTime;
using CTime3.Core.Services.CTime.ImageCache;
using CTime3.Core.Services.CTime.RequestCache;
using CTime3.Core.Services.Statistics;
using CTime3.Core.Services.Storage;
using CTime3.Core.ViewModels.Login;
using CTime3.Core.ViewModels.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace CTime3.Core;

public static class ServiceCollectionExtensions
{
    public static void AddCTimeServices(this IServiceCollection self, Action<CTimeApplicationOptions> configure)
    {
        Guard.IsNotNull(self);
        Guard.IsNotNull(configure);

        // Logging
        self.AddLogging();

        // Services
        self.AddSingleton<ICTimeService, CTimeService>();
        self.AddSingleton<ICTimeRequestCache, CTimeRequestCache>();
        self.AddSingleton<IClock, RealtimeClock>();
        self.AddSingleton<IMessenger, WeakReferenceMessenger>();
        self.AddSingleton<IEmployeeImageCache, EmployeeImageCache>();
        self.AddSingleton<IAnalyticsService, NullAnalyticsService>();
        self.AddSingleton<IConfigurationService, ConfigurationService>();
        self.AddSingleton<IStorageService, LiteDBStorageService>();
        self.AddSingleton<IStatisticsService, StatisticsService>();
        self.AddSingleton<IAlertService, NullAlertService>();

        // ViewModels
        self.AddTransient<LoginViewModel>();
        self.AddTransient<SettingsViewModel>();

        // Options
        self.AddOptions<CTimeApplicationOptions>()
            .Configure(configure)
            .Validate(o =>
                {
                    if (string.IsNullOrWhiteSpace(o.CompanyName))
                        return false;

                    if (string.IsNullOrWhiteSpace(o.AppName))
                        return false;

                    if (string.IsNullOrWhiteSpace(o.CTimeApiAppGuid))
                        return false;

                    if (string.IsNullOrWhiteSpace(o.CTimeApiBaseUrl))
                        return false;

                    if (string.IsNullOrWhiteSpace(o.CTimeImageUrlFormat))
                        return false;

                    return true;
                },
                $"The {nameof(CTimeApplicationOptions.CompanyName)} and the {nameof(CTimeApplicationOptions.AppName)} must be set.");
    }
}
