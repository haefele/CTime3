using CTime3.Apps.CommandLine.Infrastructure;
using CTime3.Core.Services.Configurations;
using CTime3.Core.Services.CTime;
using Spectre.Console;
using Spectre.Console.Cli;
using Color = Spectre.Console.Color;

namespace CTime3.Apps.CommandLine.Commands;

public class ListUsersCommand : AuthorizedCommand
{
    private readonly IAnsiConsole _ansiConsole;
    private readonly ICTimeService _cTimeService;

    public ListUsersCommand(IConfigurationService configurationService, IAnsiConsole ansiConsole, ICTimeService cTimeService)
        : base(configurationService, ansiConsole)
    {
        Guard.IsNotNull(configurationService);
        Guard.IsNotNull(ansiConsole);
        Guard.IsNotNull(cTimeService);

        this._ansiConsole = ansiConsole;
        this._cTimeService = cTimeService;
    }

    protected override async Task<int> ExecuteAuthorizedAsync(CurrentUser user, CommandContext context)
    {
        var attendanceList = await this._cTimeService.GetAttendingUsers(user.CompanyId, defaultImage: null);

        var groups = attendanceList
            .GroupBy(f => new { Name = f.AttendanceState.Name ?? "unknown", f.AttendanceState.IsAttending })
            .OrderBy(f => f.Key.IsAttending)
            .ThenBy(f => f.Key.Name);

        foreach (var group in groups)
        {
            var dayRule = new Rule($"[white underline]{group.Key.Name}[/]")
                .Border(BoxBorder.Square)
                .RuleStyle(new Style(group.Key.IsAttending ? Color.Green : Color.Red));
            this._ansiConsole.Write(dayRule);

            foreach (var attendingUser in group.OrderBy(f => f.Name))
            {
                this._ansiConsole.MarkupLine($"[bold]{attendingUser.Name}[/] {attendingUser.FirstName}");
            }
        }

        return ExitCodes.Ok;
    }
}
