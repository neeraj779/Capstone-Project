using System.Security.Claims;

namespace Swar.API.Helpers
{
    public static class UserHelper
    {
        public static int GetUserId(ClaimsPrincipal user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            string claimUserId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(claimUserId))
                throw new InvalidOperationException("User ID claim is missing.");

            return Convert.ToInt32(claimUserId);
        }
    }
}
