using CTime3.Core.Services.CTime;

namespace CTime3.Core.Services.Configurations
{
    public record Configuration(
        CurrentUser CurrentUser);

    public record CurrentUser(
        string Id,
        string CompanyId,
        string FirstName,
        string Name,
        string EmailAddress, 
        bool SupportsGeoLocation)
    {
        public static CurrentUser FromUser(User user)
        {
            return new(user.Id, user.CompanyId, user.FirstName, user.Name, user.Email, user.SupportsGeoLocation);
        }
    }
}