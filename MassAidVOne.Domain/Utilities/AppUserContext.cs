using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MassAidVOne.Domain.Utilities
{
    public static class AppUserContext
    {
        public static IHttpContextAccessor _httpContextAccessor;
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static long UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long userId))
                {
                    return userId;
                }
                return 0;
            }
        } 
        
        //public static long UserEmail
        //{
        //    get
        //    {
        //        var userIdClaim = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        //        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long userId))
        //        {
        //            return userId;
        //        }
        //        return 0;
        //    }
        //}
    }
}
