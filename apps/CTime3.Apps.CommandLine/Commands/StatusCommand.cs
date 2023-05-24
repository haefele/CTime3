using CTime3.Apps.CommandLine.Infrastructure;
using CTime3.Core.Services.Configurations;
using CTime3.Core.Services.CTime;
using CTime3.Core.Services.Statistics;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CTime3.Apps.CommandLine.Commands;

public class StatusCommand : AuthorizedCommand
{
    private readonly ICTimeService _cTimeService;
    private readonly IStatisticsService _statisticsService;
    private readonly IAnsiConsole _ansiConsole;

    public StatusCommand(ICTimeService cTimeService, IStatisticsService statisticsService, IAnsiConsole ansiConsole, IConfigurationService configurationService)
        : base(configurationService, ansiConsole)
    {
        Guard.IsNotNull(cTimeService);
        Guard.IsNotNull(statisticsService);
        Guard.IsNotNull(ansiConsole);
        Guard.IsNotNull(configurationService);

        this._cTimeService = cTimeService;
        this._statisticsService = statisticsService;
        this._ansiConsole = ansiConsole;
    }

    protected override async Task<int> ExecuteAuthorizedAsync(CurrentUser user, CommandContext context)
    {
        var time = await this._cTimeService.GetCurrentTime(user.Id);
        var currentTime = this._statisticsService.CalculateCurrentTime(time);

        this._ansiConsole.MarkupLine(currentTime.IsStillRunning
            ? $"Hey {user.FirstName}, you are [green]checked-in[/]!"
            : $"Hey {user.FirstName}, you are [red]checked-out[/]!");

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
