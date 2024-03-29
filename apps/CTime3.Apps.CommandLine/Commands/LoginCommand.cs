﻿using CTime3.Core.Services.Configurations;
using CTime3.Core.Services.CTime;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CTime3.Apps.CommandLine.Commands;

public class LoginCommand : AsyncCommand
{
    private readonly ICTimeService _cTimeService;
    private readonly IConfigurationService _configurationService;
    private readonly IAnsiConsole _ansiConsole;

    public LoginCommand(ICTimeService cTimeService, IConfigurationService configurationService, IAnsiConsole ansiConsole)
    {
        Guard.IsNotNull(cTimeService);
        Guard.IsNotNull(configurationService);
        Guard.IsNotNull(ansiConsole);

        this._cTimeService = cTimeService;
        this._configurationService = configurationService;
        this._ansiConsole = ansiConsole;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        if (this._configurationService.Config.CurrentUser is not null)
        {
            this._ansiConsole.MarkupLine($"You are [red]already logged in[/] as [bold]{this._configurationService.Config.CurrentUser.FirstName} {this._configurationService.Config.CurrentUser.Name}[/]!");

            if (this._ansiConsole.Confirm("Do you really want to login as a different user?") == false)
                return ExitCodes.Cancelled;
        }

        var username = this._ansiConsole.Ask<string>("Please enter your c-Time [bold]username[/]:");
        var password = this._ansiConsole.Prompt(new TextPrompt<string>("And your c-Time [bold]password[/]:")
        {
            IsSecret = true,
        });

        User? user = null;
        await this._ansiConsole.Status()
            .StartAsync("Logging in...", async _ =>
            {
                user = await this._cTimeService.Login(username, password);
            });

#pragma warning disable CA1508 // It is set above in the Status().StartAsync(...) call
        if (user is null)
#pragma warning restore CA1508
        {
            this._ansiConsole.MarkupLine("[red]Login failed![/] Please make sure you entered your [bold]username[/] and [bold]password[/] correctly.");
            return ExitCodes.Failed;
        }

        await this._configurationService.Modify(config => config with { CurrentUser = CurrentUser.FromUser(user) });

        this._ansiConsole.MarkupLine($"[green]Login successful![/] Welcome [bold]{user.FirstName} {user.Name}[/]!");
        this._ansiConsole.MarkupLine("You are now [green]logged in[/] and can use other commands like [bold]start[/], [bold]stop[/], [bold]status[/] or [bold]list[/].");
        this._ansiConsole.MarkupLine("To see a full list of possible commands execute the ctime command-line without any command.");

        return ExitCodes.Ok;
    }
}
