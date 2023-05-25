using CTime3.Apps.CommandLine.Commands;
using CTime3.Apps.CommandLine.Infrastructure;
using CTime3.Core;
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

            f.AddCommand<ListUsersCommand>("list-users")
                .WithDescription("Shows a list of all users in your company.");

            f.AddCommand<StartCommand>("start")
                .WithDescription("Starts a new timer.");

            f.AddCommand<StopCommand>("stop")
                .WithDescription("Stops the current timer.");
        });

        return await app.RunAsync(args);
    }

    private static ServiceCollection BuildServices()
    {
        var collection = new ServiceCollection();

        collection.AddCTimeServices(o =>
        {
            o.CompanyName = "haefele";
            o.AppName = "c-Time CLI";
        });

        return collection;
    }
}
