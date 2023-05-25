namespace CTime3.Core;

public class CTimeApplicationOptions
{
    public string CompanyName { get; set; } = string.Empty;
    public string AppName { get; set; } = string.Empty;
    public string CTimeApiAppGuid { get; set; } = "0C86E131-7ABB-4AC4-AA5E-29B8F00E7F2B";
    public string CTimeApiBaseUrl { get; set; } = "https://api.c-time.net/";
    public string CTimeImageUrlFormat { get; set; } = "https://image.c-time.net/{0}";

    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(this.CompanyName))
            return false;

        if (string.IsNullOrWhiteSpace(this.AppName))
            return false;

        if (string.IsNullOrWhiteSpace(this.CTimeApiAppGuid))
            return false;

        if (string.IsNullOrWhiteSpace(this.CTimeApiBaseUrl))
            return false;

        if (string.IsNullOrWhiteSpace(this.CTimeImageUrlFormat))
            return false;

        if (this.CTimeImageUrlFormat.Contains("{0}", StringComparison.OrdinalIgnoreCase) is false)
            return false;

        return true;
    }
}
