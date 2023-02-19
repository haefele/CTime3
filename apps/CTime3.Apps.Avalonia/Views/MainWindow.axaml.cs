using Avalonia.Controls;
using Avalonia.Interactivity;

namespace CTime3.Apps.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var result = await this.TaskDialog.ShowAsync(showHosted: true);
    }
}