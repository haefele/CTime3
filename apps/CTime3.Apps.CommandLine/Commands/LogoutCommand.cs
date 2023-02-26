using CTime3.Core.Services.Configurations;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CTime3.Apps.CommandLine.Commands;

public class LogoutCommand : AsyncCommand
{
    private readonly IConfigurationService _configurationService;
    private readonly IAnsiConsole _ansiConsole;

    public LogoutCommand(IConfigurationService configurationService, IAnsiConsole ansiConsole)
    {
        Guard.IsNotNull(configurationService);
        Guard.IsNotNull(ansiConsole);

        this._configurationService = configurationService;
        this._ansiConsole = ansiConsole;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        var currentUser = this._configurationService.Config.CurrentUser;

        if (currentUser is null)
        {
            this._ansiConsole.MarkupLine("You are [bold]currently not logged in[/].");
            return ExitCodes.Ok;
        }

        var logout = this._ansiConsole.Prompt(new ConfirmationPrompt($"You are currently logged in as [bold]{currentUser.FirstName} {currentUser.Name}[/]. Do you really want to logout?") { DefaultValue = false });
        if (logout == false)
            return ExitCodes.Cancelled;

        await this._configurationService.Modify(config => config with { CurrentUser = null });
        this._ansiConsole.MarkupLine($"[green]Logout successful![/]");

        return ExitCodes.Ok;
    }
}