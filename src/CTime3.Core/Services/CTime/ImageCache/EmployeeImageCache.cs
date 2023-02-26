namespace CTime3.Core.Services.CTime.ImageCache;

public class EmployeeImageCache : IEmployeeImageCache
{
    public string? ImageCacheEtag { get; set; } //TODO: Implement

    public async Task FillWithCachedImages(Dictionary<int, AttendingUser> users)
    {
        foreach (var user in users)
        {
            var cachedImage = await this.GetCachedImageAsync(user.Key);

            if (cachedImage is { Length: > 0 })
                user.Value.ImageAsPng = cachedImage;
        }
    }

    private async Task<byte[]?> GetCachedImageAsync(int employeeI3D)
    {
        var imageFileName = this.GetImageFileName(employeeI3D);
        var imagesFolder = this.GetImagesFolderAsync();
        var imageFilePath = Path.Combine(imagesFolder.FullName, imageFileName);

        if (File.Exists(imageFilePath) == false)
            return null;

        return await File.ReadAllBytesAsync(imageFilePath);
    }

    public async Task CacheImagesAsync(Dictionary<int, AttendingUser> users)
    {
        foreach (var user in users)
        {
            await this.CacheImageAsync(user.Key, user.Value.ImageAsPng);
        }
    }

    private async Task CacheImageAsync(int employeeI3D, byte[] image)
    {
        var imageFileName = this.GetImageFileName(employeeI3D);
        var imagesFolder = this.GetImagesFolderAsync();
        var imageFilePath = Path.Combine(imagesFolder.FullName, imageFileName);

        if (File.Exists(imageFilePath))
            File.Delete(imageFilePath);

        await File.WriteAllBytesAsync(imageFilePath, image);
    }

    private string GetImageFileName(int employeeI3D)
    {
        return $"AttendingUser-{employeeI3D}.png";
    }

    private DirectoryInfo GetImagesFolderAsync()
    {
        var path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "haefele",
            "CTime3",
            "ImageCache");

        var directory = new DirectoryInfo(path);

        if (directory.Exists == false)
            directory.Create();

        return directory;
    }
}
