using Avalonia.Controls;
using Avalonia.Interactivity;
using CTime3.Core;
using CTime3.Core.Services.CTime;
using Microsoft.Extensions.DependencyInjection;

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
        var user = await service.Login("haefele@c-entron.de", "");
        var attendingUsers = await service.GetAttendingUsers(user!.CompanyId, Array.Empty<byte>());


    }

    private static ICTimeService CreateCTimeService()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddCTimeServices(o =>
        {
            o.AppName = "c-Time Avalonia";
            o.CompanyName = "haefele";
        });

        var provider = serviceCollection.BuildServiceProvider();
        return provider.GetRequiredService<ICTimeService>();
    }
}
