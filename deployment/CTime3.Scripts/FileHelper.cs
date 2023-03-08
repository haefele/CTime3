using System.IO.Compression;

namespace CTime3.Scripts;

public static class FileHelper
{
    public static void DeleteDirectory(string directory)
    {
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    public static void ZipDirectory(string directory, string outputFilePath)
    {
        if (File.Exists(outputFilePath))
            File.Delete(outputFilePath);

        var outputDirectory = Path.GetDirectoryName(outputFilePath)!;
        if (Directory.Exists(outputDirectory) == false)
            Directory.CreateDirectory(outputDirectory);

        ZipFile.CreateFromDirectory(directory, outputFilePath);
    }

    public static void RenameFile(string directory, string fileName, string newFileName)
    {
        File.Move(Path.Combine(directory, fileName), Path.Combine(directory, newFileName));
    }
}
