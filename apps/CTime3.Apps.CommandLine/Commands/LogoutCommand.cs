using CTime3.Apps.CommandLine.Infrastructure;
using CTime3.Core.Services.Configurations;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CTime3.Apps.CommandLine.Commands;

public class LogoutCommand : AuthorizedCommand
{
    private readonly IConfigurationService _configurationService;
    private readonly IAnsiConsole _ansiConsole;

    public LogoutCommand(IConfigurationService configurationService, IAnsiConsole ansiConsole)
        : base(configurationService, ansiConsole)
    {
        Guard.IsNotNull(configurationService);
        Guard.IsNotNull(ansiConsole);

        this._configurationService = configurationService;
        this._ansiConsole = ansiConsole;
    }

    protected override async Task<int> ExecuteAuthorizedAsync(CurrentUser user, CommandContext context)
    {
        var logout = this._ansiConsole.Prompt(new ConfirmationPrompt($"You are currently logged in as [bold]{user.FirstName} {user.Name}[/]. Do you really want to logout?") { DefaultValue = false });
        if (logout == false)
            return ExitCodes.Cancelled;

        await this._configurationService.Modify(config => config with { CurrentUser = null });
        this._ansiConsole.MarkupLine($"[green]Logout successful![/]");

        return ExitCodes.Ok;
    }
}
