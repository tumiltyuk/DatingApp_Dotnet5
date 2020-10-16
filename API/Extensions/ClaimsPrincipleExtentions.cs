using System.Security.Claims;

namespace API.Extensions
{
    public static class ClaimsPrincipleExtentions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            // Get User
            // Get username from the token used to authenticate the user passed by the api
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        }
    }
}