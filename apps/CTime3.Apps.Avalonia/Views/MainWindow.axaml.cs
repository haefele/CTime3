using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using CTime3.Core.Services.Analytics;
using CTime3.Core.Services.Clock;
using CTime3.Core.Services.CTime;
using CTime3.Core.Services.CTime.ImageCache;
using CTime3.Core.Services.CTime.RequestCache;
using CTime3.Core.Services.GeoLocation;
using Microsoft.Extensions.Logging;

namespace CTime3.Apps.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var service = CreateCTimeService();
        var user = await service.Login("haefele@c-entron.de", "Start1234!");
        var attendingUsers = await service.GetAttendingUsers(user!.CompanyId, Array.Empty<byte>());


    }

    private static CTimeService CreateCTimeService()
    {
        var logger = new Logger<CTimeService>(new LoggerFactory(Enumerable.Empty<ILoggerProvider>()));
        var realtimeClock = new RealtimeClock();
        var requestCache = new CTimeRequestCache(realtimeClock);
        var messenger = WeakReferenceMessenger.Default;

        var employeeImageCache = new EmployeeImageCache();
        var geoLocationService = new GeoLocationService();
        var nullAnalyticsService = new NullAnalyticsService();

        var service = new CTimeService(
            logger,
            requestCache,
            messenger,
            employeeImageCache,
            geoLocationService,
            realtimeClock,
            nullAnalyticsService);

        return service;
    }
}