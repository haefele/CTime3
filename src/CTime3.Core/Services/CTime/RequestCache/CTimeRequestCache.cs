using CTime3.Core.Services.Clock;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;

namespace CTime3.Core.Services.CTime.RequestCache;

public class CTimeRequestCache : ICTimeRequestCache
{
    private static readonly TimeSpan s_cacheDuration = TimeSpan.FromMinutes(1);

    private readonly IClock _clock;

    private readonly ConcurrentDictionary<string, ValueHolder> _cache;

    public CTimeRequestCache(IClock clock)
    {
        Guard.IsNotNull(clock);

        this._clock = clock;

        this._cache = new ConcurrentDictionary<string, ValueHolder>();
    }

    public void Cache(string function, Dictionary<string, string> data, string response)
    {
        Guard.IsNotNullOrWhiteSpace(function);
        Guard.IsNotNull(data);

        var key = this.ComputeKey(function, data);
        var holder = ValueHolder.Create(response, this._clock.Now());

        this._cache.AddOrUpdate(key, holder, (_, _) => holder);
    }

    public bool TryGetCached(string function, Dictionary<string, string> data, out string? response)
    {
        Guard.IsNotNullOrWhiteSpace(function);
        Guard.IsNotNull(data);

        response = null;

        var key = this.ComputeKey(function, data);

        if (this._cache.TryGetValue(key, out var holder) == false)
            return false;

        if (holder.Time.Add(s_cacheDuration) <= this._clock.Now())
        {
            //Value timed out, remove it from the cache
            this._cache.TryRemove(key, out _);
            return false;
        }

        response = holder.CachedValue;
        return true;
    }

    public void Clear()
    {
        this._cache.Clear();
    }

    private string ComputeKey(string function, Dictionary<string, string> data)
    {
        Guard.IsNotNullOrWhiteSpace(function);
        Guard.IsNotNull(data);

        var keyObject = JObject.FromObject(new
        {
            Function = function,
            Data = data
        });

        return keyObject.ToString();
    }

    // TODO: Turn into record
    private class ValueHolder
    {
        public static ValueHolder Create(string value, DateTimeOffset time)
        {
            return new ValueHolder
            {
                CachedValue = value,
                Time = time
            };
        }

        public string CachedValue { get; private set; } = string.Empty;
        public DateTimeOffset Time { get; private set; }
    }
}
