using CliWrap;
using CliWrap.Buffered;
using static Bullseye.Targets;
using static CTime3.Scripts.FileHelper;
using static CTime3.Scripts.CTimePaths;

Target("clean", () =>
{
    DeleteDirectory(ArtifactsDirectory);
    Console.WriteLine("Cleaned artifacts directory");

    foreach (var directory in BinAndObjDirectories)
    {
        DeleteDirectory(directory);
    }
    Console.WriteLine("Cleaned bin and obj directories");
});

Target("build-cli", DependsOn("clean"), () =>
{
    var buildResult = Cli.Wrap("dotnet")
        .WithArguments(f => f
            .Add("publish")
            .Add(CTime3AppCommandLine.ProjectDirectory)
            .Add("-r").Add(CTime3AppCommandLine.Runtime)
            .Add("--self-contained")
            .Add("-c").Add("Release")
            .Add("-f").Add(CTime3AppCommandLine.TargetFramework)
            .Add("/p:PublishSingleFile=true,IncludeNativeLibrariesForSelfExtract=true"))
        .WithValidation(CommandResultValidation.None)
        .ExecuteBufferedAsync().GetAwaiter().GetResult();

    if (buildResult.ExitCode is not 0)
        throw new Exception($"Failed to build CTime3.Apps.CommandLine. Exit code: {buildResult.ExitCode}, Output: {buildResult.StandardOutput}, Error: {buildResult.StandardError}");

    Console.WriteLine("Built CTime3.Apps.CommandLine");

    RenameFile(CTime3AppCommandLine.PublishDirectory, "CTime3.Apps.CommandLine.exe", "ctime.exe");
    Console.WriteLine("Renamed files");

    ZipDirectory(CTime3AppCommandLine.PublishDirectory, CTime3AppCommandLine.ArtifactsFile);
    Console.WriteLine("Zipped artifact");
});

Target("build-wpf", DependsOn("clean"), () =>
{
    var buildResult = Cli.Wrap("dotnet")
        .WithArguments(f => f
            .Add("publish")
            .Add(CTime3AppWPF.ProjectDirectory)
            .Add("-r").Add(CTime3AppWPF.Runtime)
            .Add("--self-contained")
            .Add("-c").Add("Release")
            .Add("-f").Add(CTime3AppWPF.TargetFramework)
            .Add("/p:PublishSingleFile=true,IncludeNativeLibrariesForSelfExtract=true"))
        .WithValidation(CommandResultValidation.None)
        .ExecuteBufferedAsync().GetAwaiter().GetResult();

    if (buildResult.ExitCode is not 0)
        throw new Exception($"Failed to build CTime3.Apps.WPF. Exit code: {buildResult.ExitCode}, Output: {buildResult.StandardOutput}, Error: {buildResult.StandardError}");

    Console.WriteLine("Built CTime3.Apps.WPF");

    RenameFile(CTime3AppWPF.PublishDirectory, "CTime3.Apps.WPF.exe", "c-Time Fluent.exe");
    Console.WriteLine("Renamed files");

    ZipDirectory(CTime3AppWPF.PublishDirectory, CTime3AppWPF.ArtifactsFile);
    Console.WriteLine("Zipped artifact");
});


Target("build", DependsOn("build-cli", "build-wpf"));

Target("default", DependsOn("build"));

RunTargetsAndExit(args);
