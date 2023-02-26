namespace CTime3.Core.Services.Paths;

public interface ICTimePaths
{
    string DataDirectory { get; }
}

public class CTimePaths : ICTimePaths
{
    public CTimePaths(string applicationName)
    {
        this.DataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "haefele",
            applicationName);

        Directory.CreateDirectory(this.DataDirectory);
    }

    public string DataDirectory { get; }
}