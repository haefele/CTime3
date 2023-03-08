namespace CTime3.Core.Services.CTime.ImageCache;

public interface IEmployeeImageCache
{
    string? ImageCacheEtag { get; set; }

    Task FillWithCachedImages(List<AttendingUser> users);
    Task CacheImagesAsync(List<AttendingUser> users);
}