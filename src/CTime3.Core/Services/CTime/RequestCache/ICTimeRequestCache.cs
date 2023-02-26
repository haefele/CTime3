namespace CTime3.Core.Services.CTime.RequestCache;

public interface ICTimeRequestCache
{
    void Cache(string function, Dictionary<string, string> data, string response);
    bool TryGetCached(string function, Dictionary<string, string> data, out string? response);
    void Clear();
}