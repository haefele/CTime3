using System.Threading.Tasks;
using CTime3.Core;
using CTime3.Core.Services.Configurations;
using CTime3.Core.Services.CTime;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CTime3.Apps.CommandLine.Commands
{
    public class LogoutCommand : AsyncCommand
    {
        private readonly IConfigurationService _configurationService;
        private readonly IAnsiConsole _ansiConsole;

        public LogoutCommand(IConfigurationService configurationService, IAnsiConsole ansiConsole)
        {
            Guard.NotNull(configurationService, nameof(configurationService));
            Guard.NotNull(ansiConsole, nameof(ansiConsole));
            
            this._configurationService = configurationService;
            this._ansiConsole = ansiConsole;
        }

        public override async Task<int> ExecuteAsync(CommandContext context)
        {
            var currentUser = this._configurationService.Config.CurrentUser;
            
            if (currentUser == null)
            {
                this._ansiConsole.MarkupLine("You are [bold]currently not logged in[/].");
                return ExitCodes.Ok;
            }

            bool logout = this._ansiConsole.Prompt(new ConfirmationPrompt($"You are currently logged in as [bold]{currentUser.FirstName} {currentUser.Name}[/]. Do you really want to logout?") { DefaultValue = false });
            if (logout == false)
                return ExitCodes.Cancelled;

            await this._configurationService.Modify(config => config with {CurrentUser = null});
            this._ansiConsole.MarkupLine($"[green]Logout successful![/]");
            
            return ExitCodes.Ok;
        }
    }
}