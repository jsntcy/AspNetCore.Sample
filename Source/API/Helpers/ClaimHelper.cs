using System;

namespace AspNetCore.Sample.API.Helpers
{
    public static class ClaimHelper
    {
        public static string ExtractUserIdFromClaim(string claimValue)
        {
            if (!string.IsNullOrEmpty(claimValue) && claimValue.Contains("=", StringComparison.OrdinalIgnoreCase))
            {
                var parts = claimValue.Split('=');
                if (parts.Length == 2)
                {
                    return parts[1];
                }
            }

            return claimValue;
        }
    }
}
