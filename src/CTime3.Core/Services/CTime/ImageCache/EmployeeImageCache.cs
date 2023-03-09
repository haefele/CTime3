using CTime3.Core.Services.Storage;

namespace CTime3.Core.Services.CTime.ImageCache;

public class EmployeeImageCache : IEmployeeImageCache
{
    private readonly IStorageService _storageService;

    public string? ImageCacheEtag
    {
        get => this._storageService.GetAsync<string>("image_cache_etag", 1).Result;
        set => this._storageService.StoreAsync("image_cache_etag", 1, value).Wait();
    }

    public EmployeeImageCache(IStorageService storageService)
    {
        Guard.IsNotNull(storageService);

        this._storageService = storageService;
    }

    public async Task FillWithCachedImages(List<AttendingUser> users)
    {
        Guard.IsNotNull(users);

        foreach (var user in users)
        {
            var cachedImage = await this._storageService.GetAsync<byte[]>("image_cache", user.Id);

            if (cachedImage is { Length: > 0 })
                user.ImageAsPng = cachedImage;
        }
    }

    public async Task CacheImagesAsync(List<AttendingUser> users)
    {
        Guard.IsNotNull(users);

        foreach (var user in users)
        {
            await this._storageService.StoreAsync("image_cache", user.Id, user.ImageAsPng);
        }
    }
}
