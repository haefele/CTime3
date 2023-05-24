using System.Globalization;
using CTime3.Apps.CommandLine.Infrastructure;
using CTime3.Core.Services.Configurations;
using CTime3.Core.Services.CTime;
using Spectre.Console;
using Spectre.Console.Cli;
using Color = Spectre.Console.Color;

namespace CTime3.Apps.CommandLine.Commands;

public class ListCommand : AuthorizedCommand
{
    private readonly ICTimeService _cTimeService;
    private readonly IAnsiConsole _ansiConsole;

    public ListCommand(ICTimeService cTimeService, IConfigurationService configurationService, IAnsiConsole ansiConsole)
        : base(configurationService, ansiConsole)
    {
        Guard.IsNotNull(cTimeService);
        Guard.IsNotNull(ansiConsole);

        this._cTimeService = cTimeService;
        this._ansiConsole = ansiConsole;
    }

    protected override async Task<int> ExecuteAuthorizedAsync(CurrentUser user, CommandContext context)
    {
        foreach (var (start, end) in this.GetTimes())
        {
            var times = new List<Time>();
            await this._ansiConsole
                .Status()
                .StartAsync("Loading times", async _ =>
                {
                    times = await this._cTimeService.GetTimes(user.Id, start, end);
                });

            this.RenderTimes(times);
        }

        return ExitCodes.Ok;
    }

    private IEnumerable<(DateTime start, DateTime end)> GetTimes()
    {
        var start = DateTime.Today.AddDays(-6);
        var end = DateTime.Today;

        while (true)
        {
            yield return (start, end);

            if (this._ansiConsole.Confirm("Show more?") == false)
                yield break;

            end = start.AddDays(-1);
            start = start.AddDays(-7);
        }
    }

    private void RenderTimes(IList<Time> times)
    {
        var timesPerDay = from time in times
                          orderby time.Day descending
                          group time by time.Day
                          into g
                          select new
                          {
                              Day = g.Key,
                              Times = g.OrderByDescending(f => f.ClockInTime)
                          };

        foreach (var perDay in timesPerDay)
        {
            var hasAnyTimes = perDay.Times.Any(f => f.ClockInTime is not null || f.ClockOutTime is not null);

            var dayRule = new Rule($"[white underline]{perDay.Day:ddd dd. MMM yyyy}[/] [white dim]({(int)perDay.Times.First().Hours.TotalHours:00}:{perDay.Times.First().Hours.Minutes:00})[/]")
                .Border(IsWeekend(perDay.Day) ? BoxBorder.Double : BoxBorder.Square)
                .RuleStyle(new Style(hasAnyTimes ? Color.Green : Color.Red));
            this._ansiConsole.Write(dayRule);

            if (hasAnyTimes == false)
            {
                this._ansiConsole.MarkupLine($"[dim]No times on this day.[/]");
                this._ansiConsole.WriteLine();
            }
            else
            {
                var titleLength = Markup.Remove(dayRule.Title!).Length;
                var durationLength = "(00:00)".Length;
                var rulePaddingLeft = 3;

                foreach (var time in perDay.Times)
                {
                    var from = time.ClockInTime?.ToString("t", CultureInfo.CurrentCulture) ?? "?";
                    var to = time.ClockOutTime?.ToString("t", CultureInfo.CurrentCulture) ?? "?";
                    var duration = time.ClockInTime is not null && time.ClockOutTime is not null
                        ? time.ClockOutTime.Value - time.ClockInTime.Value
                        : (TimeSpan?)null;

                    var timeAsString = $"{@from} - {to}".PadRight(titleLength + rulePaddingLeft - durationLength);
                    if (duration is not null)
                        timeAsString += $"[dim]({(int)duration.Value.TotalHours:00}:{duration.Value.Minutes:00})[/]";

                    this._ansiConsole.MarkupLine(timeAsString);
                }

                this._ansiConsole.WriteLine();
            }
        }
    }

    private bool IsWeekend(DateTime date)
    {
        return date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }
}
