namespace CTime3.Core.Services.ApplicationEnvironment;

public class ApplicationEnvironment : IApplicationEnvironment
{
    public ApplicationEnvironment(string companyName, string appName)
    {
        Guard.IsNotNullOrWhiteSpace(companyName);
        Guard.IsNotNullOrWhiteSpace(appName);

        this.CompanyName = companyName;
        this.AppName = appName;
    }

    public string CompanyName { get; }
    public string AppName { get; }
}
