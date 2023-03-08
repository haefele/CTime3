using System.Collections.ObjectModel;

namespace CTime3.Scripts;

public static class CTimePaths
{
    public static string SolutionDirectory
    {
        get
        {
            var currentFolder = new DirectoryInfo(Path.GetDirectoryName(typeof(CTimePaths).Assembly.Location)!);
            return currentFolder.Parent?.Parent?.Parent?.Parent?.Parent?.FullName ?? throw new InvalidOperationException("Could not determine the SolutionDirectory");
        }
    }

    public static string ArtifactsDirectory => Path.Combine(SolutionDirectory, "artifacts");

    public static ReadOnlyCollection<string> ProjectDirectories => new[]
        {
            CTime3Core.ProjectDirectory,
            CTime3AppAvalonia.ProjectDirectory,
            CTime3AppCommandLine.ProjectDirectory,
            CTime3AppWPF.ProjectDirectory,
        }.ToList().AsReadOnly();

    public static ReadOnlyCollection<string> BinAndObjDirectories => new[]
        {
            ProjectDirectories.Select(f => Path.Combine(f, "bin")),
            ProjectDirectories.Select(f => Path.Combine(f, "obj")),
        }.SelectMany(f => f).ToList().AsReadOnly();

    public static class CTime3Core
    {
        public static string ProjectDirectory => Path.Combine(SolutionDirectory, "src", "CTime3.Core");
    }

    public static class CTime3AppAvalonia
    {
        public static string ProjectDirectory => Path.Combine(SolutionDirectory, "apps", "CTime3.Apps.Avalonia");
    }

    public static class CTime3AppCommandLine
    {
        public static string Runtime => "win-x64";
        public static string TargetFramework => "net7.0";

        public static string ProjectDirectory => Path.Combine(SolutionDirectory, "apps", "CTime3.Apps.CommandLine");
        public static string PublishDirectory => Path.Combine(ProjectDirectory, "bin", "Release", TargetFramework, Runtime, "publish");
        public static string ArtifactsFile => Path.Combine(ArtifactsDirectory, "CTime3.Apps.CommandLine.zip");
    }

    public static class CTime3AppWPF
    {
        public static string Runtime => "win-x64";
        public static string TargetFramework => "net7.0-windows";

        public static string ProjectDirectory => Path.Combine(SolutionDirectory, "apps", "CTime3.Apps.WPF");
        public static string PublishDirectory => Path.Combine(ProjectDirectory, "bin", "Release", TargetFramework, Runtime, "publish");
        public static string ArtifactsFile => Path.Combine(ArtifactsDirectory, "CTime3.Apps.WPF.zip");
    }
}
