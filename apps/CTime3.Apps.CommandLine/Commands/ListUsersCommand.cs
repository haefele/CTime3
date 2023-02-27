using CTime3.Core.Services.Configurations;
using CTime3.Core.Services.CTime;
using Spectre.Console;
using Spectre.Console.Cli;
using Color = Spectre.Console.Color;

namespace CTime3.Apps.CommandLine.Commands;

public class ListUsersCommand : AsyncCommand
{
    private readonly IConfigurationService _configurationService;
    private readonly IAnsiConsole _ansiConsole;
    private readonly ICTimeService _cTimeService;

    public ListUsersCommand(IConfigurationService configurationService, IAnsiConsole ansiConsole, ICTimeService cTimeService)
    {
        Guard.IsNotNull(configurationService);
        Guard.IsNotNull(ansiConsole);
        Guard.IsNotNull(cTimeService);

        this._configurationService = configurationService;
        this._ansiConsole = ansiConsole;
        this._cTimeService = cTimeService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        var currentUser = this._configurationService.Config.CurrentUser;

        if (currentUser is null)
        {
            this._ansiConsole.MarkupLine("You are [bold]currently not logged in[/].");
            return ExitCodes.Ok;
        }

        var attendanceList = await this._cTimeService.GetAttendingUsers(currentUser.CompanyId, defaultImage: null);

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

            foreach (var user in group.OrderBy(f => f.Name))
            {
                this._ansiConsole.MarkupLine($"[bold]{user.Name}[/] {user.FirstName}");
            }
        }

        return ExitCodes.Ok;
    }
}
