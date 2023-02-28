using CliWrap;
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
    Cli.Wrap("dotnet")
        .WithArguments(f => f
            .Add("publish")
            .Add(CTime3AppCommandLine.ProjectDirectory)
            .Add("-r").Add(CTime3AppCommandLine.Runtime)
            .Add("--self-contained")
            .Add("-c").Add("Release")
            .Add("-f").Add(CTime3AppCommandLine.TargetFramework)
            .Add("/p:PublishSingleFile=true,IncludeNativeLibrariesForSelfExtract=true"))
        .ExecuteAsync().GetAwaiter().GetResult();
    Console.WriteLine("Built CTime3.Apps.CommandLine");

    RenameFile(CTime3AppCommandLine.PublishDirectory, "CTime3.Apps.CommandLine.exe", "ctime.exe");
    Console.WriteLine("Renamed files");

    ZipDirectory(CTime3AppCommandLine.PublishDirectory, CTime3AppCommandLine.ArtifactsFile);
    Console.WriteLine("Zipped artifact");
});

Target("build", DependsOn("build-cli"));

Target("default", DependsOn("build"));

RunTargetsAndExit(args);
