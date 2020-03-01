namespace JwtBearMemoryLeak.Authentications
{
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Linq;
    using System.Security.Claims;

    public static class UserInfoServiceExtensions
    {
        public static void Bind(this IUserInfo userInfo, HttpContext context, ClaimsPrincipal principal)
        {
            var userId = Convert.ToInt32(principal.Claims.First(cw => cw.Type == ClaimTypes.NameIdentifier).Value);
            var userName = principal.Identity.Name?? principal.Claims.FirstOrDefault(cw => cw.Type == ClaimTypes.NameIdentifier)?.Value;

            userInfo.SetUser(userId, userName);
        }
    }
}
