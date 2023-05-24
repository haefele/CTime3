using CTime3.Core.Services.Configurations;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CTime3.Apps.CommandLine.Infrastructure;

public abstract class AuthorizedCommand : AsyncCommand
{
    private readonly IConfigurationService _configurationService;
    private readonly IAnsiConsole _ansiConsole;

    protected AuthorizedCommand(IConfigurationService configurationService, IAnsiConsole ansiConsole)
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

        return await this.ExecuteAuthorizedAsync(currentUser, context);
    }

    protected abstract Task<int> ExecuteAuthorizedAsync(CurrentUser user, CommandContext context);
}
