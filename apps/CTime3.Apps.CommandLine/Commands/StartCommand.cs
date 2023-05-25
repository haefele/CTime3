using CTime3.Apps.CommandLine.Infrastructure;
using CTime3.Core.Services.Configurations;
using CTime3.Core.Services.CTime;
using CTime3.Core.Services.Statistics;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CTime3.Apps.CommandLine.Commands;

public class StartCommand : AuthorizedCommand
{
    private readonly IAnsiConsole _ansiConsole;
    private readonly ICTimeService _ctimeService;
    private readonly IStatisticsService _statisticsService;

    public StartCommand(IConfigurationService configurationService, IAnsiConsole ansiConsole, ICTimeService ctimeService, IStatisticsService statisticsService)
        : base(configurationService, ansiConsole)
    {
        Guard.IsNotNull(ansiConsole);
        Guard.IsNotNull(ctimeService);
        Guard.IsNotNull(statisticsService);

        this._ansiConsole = ansiConsole;
        this._ctimeService = ctimeService;
        this._statisticsService = statisticsService;
    }

    protected override async Task<int> ExecuteAuthorizedAsync(CurrentUser user, CommandContext context)
    {
        var time = await this._ctimeService.GetCurrentTime(user.Id);
        var currentTime = this._statisticsService.CalculateCurrentTime(time);

        if (currentTime.IsStillRunning)
        {
            this._ansiConsole.MarkupLine($"You are already [green]checked-in[/] since [bold]{time!.ClockInTime!:HH\\:mm}[/] (for {currentTime.WorkTime:hh\\:mm})!");
            return ExitCodes.Failed;
        }

        await this._ctimeService.SaveTimer(user, DateTime.Now, TimeState.Entered);
        this._ansiConsole.MarkupLine($"Done! You are now [green]checked-in[/]!");
        return ExitCodes.Ok;
    }
}
