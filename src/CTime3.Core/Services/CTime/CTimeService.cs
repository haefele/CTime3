using CTime3.Core.Resources;
using CTime3.Core.Services.Analytics;
using CTime3.Core.Services.Clock;
using CTime3.Core.Services.CTime.RequestCache;
using CTime3.Core.Services.GeoLocation;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Polly;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;
using CTime3.Core.Services.CTime.ImageCache;

using static CTime3.Core.Extensions.StringExtensions;
using static CTime3.Core.Extensions.JsonExtensions;

namespace CTime3.Core.Services.CTime
{
    public class CTimeService : ICTimeService, IDisposable
    {
        private const string CTimeUniversalAppGuid = "0C86E131-7ABB-4AC4-AA5E-29B8F00E7F2B";

        private readonly ILogger<CTimeService> _logger;
        private readonly ICTimeRequestCache _requestCache;
        private readonly IMessenger _messenger;
        private readonly IEmployeeImageCache _employeeImageCache;
        private readonly IGeoLocationService _geoLocationService;
        private readonly IClock _clock;
        private readonly IAnalyticsService _analyticsService;
        private readonly HttpClient _client;

        public CTimeService(ILogger<CTimeService> logger, ICTimeRequestCache requestCache, IMessenger messenger, IEmployeeImageCache employeeImageCache, IGeoLocationService geoLocationService, IClock clock, IAnalyticsService analyticsService)
        {
            Guard.IsNotNull(logger);
            Guard.IsNotNull(requestCache);
            Guard.IsNotNull(messenger);
            Guard.IsNotNull(employeeImageCache);
            Guard.IsNotNull(geoLocationService);
            Guard.IsNotNull(clock);
            Guard.IsNotNull(analyticsService);

            this._logger = logger;
            this._requestCache = requestCache;
            this._messenger = messenger;
            this._employeeImageCache = employeeImageCache;
            this._geoLocationService = geoLocationService;
            this._clock = clock;
            this._analyticsService = analyticsService;
            this._client = new HttpClient();
        }

        public async Task<User?> Login(string emailAddress, string password)
        {
            try
            {
                var data = new Dictionary<string, string>
                {
                    {"Password", this.GetHashedPassword(password)},
                    {"LoginName", emailAddress},
                    {"Crypt", 1.ToString()},
                    {"APPGUID", CTimeUniversalAppGuid },
                };
                var responseJson = await this.SendRequestAsync("LoginV2.php", data, canBeCached: false);

                var user = responseJson
                    ?.Value<JArray>("Result")
                    ?.Select(f => f.ToObject<JObject>())
                    .FirstOrDefault();

                if (user is null)
                    return null;

                return new User
                {
                    Id = user.Value<string>("EmployeeGUID")!,
                    CompanyId = user.Value<string>("CompanyGUID")!,
                    Email = user.Value<string>("LoginName"),
                    FirstName = user.Value<string>("EmployeeFirstName"),
                    Name = user.Value<string>("EmployeeName"),
                    ImageAsPng = user.ValueAsBase64Array("EmployeePhoto"),
                    SupportsGeoLocation = user.Value<int>("GeolocationAllowed") == 1,
                    CompanyImageAsPng = user.ValueAsBase64Array("CompanyImage"),
                };
            }
            catch (Exception exception) when (exception is CTimeException == false)
            {
                this._logger.LogWarning(exception, "Exception in method {Method}. Email address: {EmailAddress}", nameof(Login), emailAddress);
                this._analyticsService.TrackException(exception);

                throw new CTimeException(Messages.CTimeService_ErrorWhileLogin, exception);
            }
        }

        public async Task<List<Time>> GetTimes(string employeeGuid, DateTime start, DateTime end)
        {
            try
            {
                var responseJson = await this.SendRequestAsync("GetTimerListV2.php", new Dictionary<string, string>
                {
                    {"EmployeeGUID", employeeGuid},
                    {"DateTill", end.ToString("yyyy-MM-dd")},
                    {"DateFrom", start.ToString("yyyy-MM-dd")},
                    {"Summary", 1.ToString()},
                    {"APPGUID", CTimeUniversalAppGuid },
                });

                if (responseJson?.Value<JArray>("Result") is null)
                    return new List<Time>();

                return responseJson
                    .Value<JArray>("Result")!
                    .Cast<JObject>()
                    .Select(f => new Time
                    {
                        Day = f.Value<DateTime>("DayDate"),
                        Hours = f.ValueAsTimeSpan("TimeHour_IST_HR"),
                        State = (TimeState?)f.Value<int?>("TimeTrackTypePure"),
                        StateDescription = f.Value<string>("TimeTrackTypeDescription")!,
                        ClockInTime = f.Value<DateTime?>("TimeTrackIn"),
                        ClockOutTime = f.Value<DateTime?>("TimeTrackOut"),
                    })
                    .Select(f =>
                    {
                        if (f.ClockInTime is not null && f.ClockOutTime is not null)
                        {
                            f.State = (f.State ?? 0) | TimeState.Left;
                        }
                        else if (f.ClockInTime is not null)
                        {
                            f.State = (f.State ?? 0) | TimeState.Entered;
                        }

                        if (f.State is TimeState.Entered or TimeState.Left)
                        {
                            f.StateDescription = null;
                        }

                        return f;
                    })
                    .Where(f => f.Day <= this._clock.Today() || f.ClockInTime is not null || f.ClockOutTime is not null)
                    .ToList();
            }
            catch (Exception exception) when (exception is CTimeException == false)
            {
                this._logger.LogWarning(exception, "Exception in method {Method}. Employee: {EmployeeGuid}, Start: {Start}, End: {End}", nameof(this.GetTimes), employeeGuid, start, end);

                this._analyticsService.TrackException(exception);

                throw new CTimeException(Messages.CTimeService_ErrorWhileLoadingTimes, exception);
            }
        }

        public async Task SaveTimer(string employeeGuid, string? rfidKey, DateTime time, string companyId, TimeState state, bool withGeolocation)
        {
            try
            {
                var location = withGeolocation
                    ? await this._geoLocationService.TryGetGeoLocationAsync()
                    : null;

                var data = new Dictionary<string, string>
                {
                    {"TimerKind", ((int) state).ToString()},
                    {"TimerText", string.Empty},
                    {"TimerTime", time.ToString("yyyy-MM-dd HH:mm:ss")},
                    {"EmployeeGUID", employeeGuid},
                    {"GUID", companyId},
                    {"RFID", rfidKey ?? string.Empty},
                    {"lat", location?.Latitude.ToString(CultureInfo.InvariantCulture) ?? string.Empty },
                    {"long", location?.Longitude.ToString(CultureInfo.InvariantCulture) ?? string.Empty },
                    {"APPGUID", CTimeUniversalAppGuid },
                };

                var responseJson = await this.SendRequestAsync("SaveTimerV2.php", data, canBeCached: false);

                if (string.IsNullOrWhiteSpace(responseJson?.Value<string>("Greeting")))
                    throw new CTimeException(Messages.CTimeService_ErrorWhileStamp);

                //Make sure to clear the cache before we fire the UserStamped event
                this._requestCache.Clear();

                this._messenger.Send(new UserStampedMessage());
            }
            catch (Exception exception) when (exception is CTimeException == false)
            {
                this._logger.LogWarning(exception, "Exception in method {Method}. Employee: {EmployeeGuid}, Time: {Time}, Company Id: {CompanyId}, State: {State}", nameof(this.SaveTimer), employeeGuid, time, companyId, state);
                this._analyticsService.TrackException(exception);

                throw new CTimeException(Messages.CTimeService_ErrorWhileStamp, exception);
            }
        }

        public async Task<Time?> GetCurrentTime(string employeeGuid)
        {
            try
            {
                IList<Time> timesForToday = await this.GetTimes(employeeGuid, this._clock.Today().AddDays(-1), this._clock.Today());

                bool IsFinishedTimeInFuture(Time time)
                {
                    if (time.ClockOutTime is null)
                        return false;

                    if (time.ClockOutTime <= this._clock.Now().AddMinutes(5))
                        return false;

                    // Time is completed, and is further in the future than 5 minutes
                    return true;
                }

                return timesForToday
                    .OrderByDescending(f => f.ClockInTime) // For example, if you have a half day off in the afternoon
                    .FirstOrDefault(f => IsFinishedTimeInFuture(f) == false);
            }
            catch (Exception exception) when (exception is CTimeException == false)
            {
                this._logger.LogWarning(exception, "Exception in method {Method}. Employee: {EmployeeGuid}", nameof(this.GetCurrentTime), employeeGuid);
                this._analyticsService.TrackException(exception);

                throw new CTimeException(Messages.CTimeService_ErrorWhileLoadingCurrentTime, exception);
            }
        }

        public async Task<List<AttendingUser>> GetAttendingUsers(string companyId, byte[]? defaultImage)
        {
            try
            {
                var currentCacheEtag = this._employeeImageCache.ImageCacheEtag;

                var responseJson = await this.SendRequestAsync("GetPresenceListV2.php", new Dictionary<string, string>
                {
                    {"GUID", companyId},
                    {"cacheDate", currentCacheEtag ?? string.Empty },
                    {"APPGUID", CTimeUniversalAppGuid },
                });

                if (responseJson is null || responseJson.Value<JArray>("Result") is null)
                    return new List<AttendingUser>();

                var newCacheEtag = responseJson
                    .Value<JArray>("Result")!
                    .Cast<JObject>()
                    .Select(f => f.Value<string>("cacheDate"))
                    .FirstOrDefault();

                var defaultImageAsBase64 = Convert.ToBase64String(defaultImage ?? Array.Empty<byte>());

                var result = responseJson
                    .Value<JArray>("Result")!
                    .Cast<JObject>()
                    .Select(f => new
                    {
                        EmployeeI3D = f.Value<int>("EmployeeI3D"),
                        Employee = new AttendingUser
                        {
                            Id = f.Value<int>("EmployeeI3D").ToString(),
                            Name = f.Value<string>("EmployeeName"),
                            FirstName = f.Value<string>("EmployeeFirstName"),
                            AttendanceState = new AttendanceState
                            {
                                IsAttending = f.Value<int>("PresenceStatus") == 1,
                                Name = this.ParseAttendanceStateName(f.Value<string>("TimerTypeDescription"), f.Value<int?>("TimeTrackTypePure"), f.Value<int>("PresenceStatus") == 1),
                                Color = this.ParseColor(f.Value<string>("EnumColor"), f.Value<int?>("TimeTrackTypePure")),
                            },
                            ImageAsPng = Convert.FromBase64String(f.Value<string>("EmployeePhoto") ?? defaultImageAsBase64),
                            EmailAddress = f.Value<string>("EmployeeEmail"),
                            PhoneNumber = f.Value<string>("EmployeePhone"),
                            Departments = f.Value<string>("EmployeeGroups")
                                           ?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                           .Select(d => d.Trim())
                                           .ToArray() ?? Array.Empty<string>(),
                        }
                    })
                    .GroupBy(f => f.EmployeeI3D)
                    .ToDictionary(f => f.Key, f => f.Select(d => d.Employee).First());

                if (newCacheEtag == currentCacheEtag)
                {
                    await this._employeeImageCache.FillWithCachedImages(result);
                }
                else
                {
                    await this._employeeImageCache.CacheImagesAsync(result);
                }

                return result.Select(f => f.Value).ToList();
            }
            catch (Exception exception) when (exception is CTimeException == false)
            {
                this._logger.LogError(exception, "Exception in method {Method}. Company Id: {CompanyId}", nameof(this.GetAttendingUsers), companyId);
                this._analyticsService.TrackException(exception);

                throw new CTimeException(Messages.CTimeService_ErrorWhileLoadingAttendanceList, exception);
            }
        }

        public void Dispose()
        {
            this._client.Dispose();
        }

        private string ParseAttendanceStateName(string? potentialName, int? state, bool attending)
        {
            if (state == (int)TimeState.Entered)
                return Messages.Entered;

            if (state is (int)TimeState.Left or null or 0)
                return Messages.Left;

            if (state == (int)TimeState.HomeOffice)
                potentialName = Messages.HomeOffice;

            if (state == (int)TimeState.ShortBreak)
                potentialName = Messages.ShortBreak;

            if (state == (int)TimeState.Trip)
                potentialName = Messages.Trip;

            var suffix = attending
                ? Messages.Entered
                : Messages.Left;

            return $"{potentialName?.MakeFirstCharacterUpperCase()} ({suffix})";
        }

        private Color ParseColor(string? color, int? state)
        {
            //For default Entered and Left we use our own red and green colors "CTimeGreen" and "CTimeRed"
            if (state == (int)TimeState.Entered)
                return Color.FromArgb(255, 63, 195, 128);

            var stateIsLeft = state == (int)TimeState.Left;
            var stateIsEmpty = state is null or 0;
            var stateIsExpected = state == -1; //There is a special -1 state, that is called "Erwartet" - you're expected to work today, but didn't start yet

            if (stateIsLeft || stateIsEmpty || stateIsExpected)
                return Color.FromArgb(255, 231, 76, 60);

            if (string.IsNullOrWhiteSpace(color))
                return Color.Transparent;

            var r = color.Substring(1, 2);
            var g = color.Substring(3, 2);
            var b = color.Substring(5, 2);

            return Color.FromArgb(
                255,
                byte.Parse(r, NumberStyles.HexNumber),
                byte.Parse(g, NumberStyles.HexNumber),
                byte.Parse(b, NumberStyles.HexNumber));
        }

        private string GetHashedPassword(string password)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var hashedPasswordBytes = MD5.HashData(passwordBytes);
            var hashedPasswordString = BitConverter.ToString(hashedPasswordBytes);

            return hashedPasswordString.Replace("-", string.Empty).ToLower();
        }

        private async Task<JObject?> SendRequestAsync(string function, Dictionary<string, string> data, bool canBeCached = true)
        {
            if (canBeCached == false || this._requestCache.TryGetCached(function, data, out var responseContentAsString) == false)
            {
                var retryPolicy = Policy
                    .Handle<Exception>()
                    .OrResult<HttpResponseMessage>(f => f.StatusCode != HttpStatusCode.OK)
                    .WaitAndRetryAsync(5, _ => TimeSpan.FromSeconds(1));

                var response = await retryPolicy.ExecuteAsync(cancellationToken =>
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, this.BuildUri(function))
                    {
                        Content = new FormUrlEncodedContent(data),
                    };

                    return this._client.SendAsync(request, cancellationToken);

                }, CancellationToken.None, continueOnCapturedContext: true);

                if (response.StatusCode != HttpStatusCode.OK)
                    return null;

                responseContentAsString = await response.Content.ReadAsStringAsync();

                if (canBeCached)
                    this._requestCache.Cache(function, data, responseContentAsString);
            }
            else
            {
                // Fake a little bit of delay before returning the cached result
                await Task.Delay(TimeSpan.FromSeconds(0.1));
            }

            return responseContentAsString is null
                ? null
                : JObject.Parse(responseContentAsString);
        }

        private Uri BuildUri(string function)
        {
            var baseUri = this.GetBaseUri();
            return new Uri($"{baseUri}{function}");
        }

        private string GetBaseUri()
        {
            return "https://api.c-time.net/";
        }
    }
}
