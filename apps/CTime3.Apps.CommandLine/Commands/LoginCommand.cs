using System;
using System.Threading.Tasks;
using CTime3.Core.Services.CTime;
using CTime3.Core.Services.DataStorage;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CTime3.Apps.CommandLine.Commands
{
    public class LoginCommand : AsyncCommand
    {
        private readonly ICTimeService _cTimeService;
        private readonly IDataStorage _dataStorage;
        private readonly IAnsiConsole _ansiConsole;

        public LoginCommand(ICTimeService cTimeService, IDataStorage dataStorage, IAnsiConsole ansiConsole)
        {
            this._cTimeService = cTimeService;
            this._dataStorage = dataStorage;
            this._ansiConsole = ansiConsole;
        }

        public override async Task<int> ExecuteAsync(CommandContext context)
        {
            var emailAddress = this._ansiConsole.Ask<string>("Please enter your c-Time [bold]email address[/]:");
            var password = this._ansiConsole.Prompt(new TextPrompt<string>("And your c-Time [bold]password[/]:")
            {
                IsSecret = true,
            });

            User user = null;
            await this._ansiConsole.Status()
                .StartAsync("Logging in...", async context =>
                {
                    user = await this._cTimeService.Login(emailAddress, password);
                });
            
            if (user == null)
            {
                this._ansiConsole.Write(new Rule("[red]Login failed[/]").LeftAligned());
                this._ansiConsole.MarkupLine("Please make sure you entered your [bold]email address[/] and [bold]password[/] correctly.");
                
                return 1;
            }

            return 0;
        }
    }
}