using CTime3.Core.Services.CTime;
using System.Threading.Tasks;

namespace CTime3.Core.Services.GeoLocation
{
    public interface IGeoLocationService
    {
        Task<GeoLocationState> GetGeoLocationStateAsync(User user);
        Task<Geopoint> TryGetGeoLocationAsync();
    }

    public class GeoLocationService : IGeoLocationService
    {
        public Task<GeoLocationState> GetGeoLocationStateAsync(User user)
        {
            return Task.FromResult(GeoLocationState.NotRequired);
        }

        public Task<Geopoint> TryGetGeoLocationAsync()
        {
            return Task.FromResult<Geopoint>(null);
        }
    }

    public enum GeoLocationState
    {
        NotRequired,
        RequiredNotAvailable,
        RequiredAndAvailable
    }

    public record Geopoint(double Latitude, double Longitude);
}
