using System;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CTime3.Core;
using CTime3.Core.Services.Configurations;
using CTime3.Core.Services.CTime;
using CTime3.Core.Services.Statistics;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CTime3.Apps.CommandLine.Commands
{
    public class StatusCommand : AsyncCommand
    {
        private readonly ICTimeService _cTimeService;
        private readonly IStatisticsService _statisticsService;
        private readonly IAnsiConsole _ansiConsole;
        private readonly IConfigurationService _configurationService;

        public StatusCommand(ICTimeService cTimeService, IStatisticsService statisticsService, IAnsiConsole ansiConsole, IConfigurationService configurationService)
        {
            Guard.IsNotNull(cTimeService);
            Guard.IsNotNull(statisticsService);
            Guard.IsNotNull(ansiConsole);
            Guard.IsNotNull(configurationService);

            this._cTimeService = cTimeService;
            this._statisticsService = statisticsService;
            this._ansiConsole = ansiConsole;
            this._configurationService = configurationService;
        }

        public override async Task<int> ExecuteAsync(CommandContext context)
        {
            if (this._configurationService.Config.CurrentUser is null)
            {
                this._ansiConsole.MarkupLine("You are not [red]logged in![/] You have to login before using this command.");
                return ExitCodes.Failed;
            }

            var time = await this._cTimeService.GetCurrentTime(this._configurationService.Config.CurrentUser.Id);
            var currentTime = this._statisticsService.CalculateCurrentTime(time);

            this._ansiConsole.MarkupLine(currentTime.IsStillRunning
                ? $"Hey {this._configurationService.Config.CurrentUser.FirstName}, you are [green]checked-in[/]!"
                : $"Hey {this._configurationService.Config.CurrentUser.FirstName}, you are [red]checked-out[/]!");

            this._ansiConsole.MarkupLine(currentTime.OverTime is null
                ? $"Your [underline]current time[/] is [bold]{currentTime.WorkTime:hh\\:mm}[/]"
                : $"Your [underline]current time[/] is [bold]{currentTime.WorkTime:hh\\:mm}[/] with a [underline]overtime[/] of [bold]{currentTime.OverTime.Value:hh\\:mm}[/]");

            if (currentTime.CurrentBreak is not null)
            {
                this._ansiConsole.MarkupLine($"You are currently on [underline]break[/] for {currentTime.CurrentBreak.BreakTime:hh\\:mm} until {currentTime.CurrentBreak.PreferredBreakTimeEnd:hh\\:mm}");
            }

            return ExitCodes.Ok;
        }
    }
}
