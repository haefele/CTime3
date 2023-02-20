using System.Collections.Generic;
using System.Threading.Tasks;

namespace CTime3.Core.Services.CTime.ImageCache
{
    public interface IEmployeeImageCache
    {
        string? ImageCacheEtag { get; set; }
        
        Task FillWithCachedImages(Dictionary<int, AttendingUser> users);
        Task CacheImagesAsync(Dictionary<int, AttendingUser> users);
    }
}