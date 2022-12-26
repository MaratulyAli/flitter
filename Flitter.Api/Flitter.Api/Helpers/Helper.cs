using System.Security.Claims;
using System.Security.Principal;

namespace Flitter.Api.Helpers
{
    public class Helper
    {
        public static string GetUserId(IIdentity identity)
        {
            return (identity as ClaimsIdentity)?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        }
    }
}
