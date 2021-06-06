using System;
using System.Threading.Tasks;
using CTime3.Core;
using CTime3.Core.Services.Configurations;
using CTime3.Core.Services.CTime;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CTime3.Apps.CommandLine.Commands
{
    public class ListCommand : AsyncCommand
    {
        private readonly ICTimeService _cTimeService;
        private readonly IConfigurationService _configurationService;
        private readonly IAnsiConsole _ansiConsole;

        public ListCommand(ICTimeService cTimeService, IConfigurationService configurationService, IAnsiConsole ansiConsole)
        {
            Guard.NotNull(cTimeService, nameof(cTimeService));
            Guard.NotNull(configurationService, nameof(configurationService));
            Guard.NotNull(ansiConsole, nameof(ansiConsole));
            
            this._cTimeService = cTimeService;
            this._configurationService = configurationService;
            this._ansiConsole = ansiConsole;
        }
        
        public override async Task<int> ExecuteAsync(CommandContext context)
        {
            if (this._configurationService.Config.CurrentUser == null)
            {
                this._ansiConsole.MarkupLine("You are not [red]logged in![/] You have to login before using this command.");
                return ExitCodes.Failed;
            }

            var times = await this._cTimeService.GetTimes(this._configurationService.Config.CurrentUser.Id, DateTime.Today.AddDays(-7), DateTime.Today);

            foreach (var time in times)
            {
                // this._ansiConsole.MarkupLine($"{time.}");
            }
            
            return ExitCodes.Ok;
        }
    }
}