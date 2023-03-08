namespace CTime3.Core.Services.Configurations;

public interface IConfigurationService
{
    Configuration Config { get; }

    Task Modify(Func<Configuration, Configuration> changeAction);
}