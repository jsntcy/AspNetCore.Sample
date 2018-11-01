using System.Linq;

using AspNetCore.Sample.API.Helpers;
using AspNetCore.Sample.Common.ErrorHandling;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Apex.Security.Constants;

namespace AspNetCore.Sample.API.Controllers
{
    public abstract class BaseController : Controller
    {
        protected string GetUserIdOrThrow()
        {
            var userId = TryGetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                return userId;
            }

            throw Errors.Unauthorized().Exception();
        }

        // Get user id for docs site users.
        protected string TryGetUserId()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var docsUserIdClaim = User.Claims.FirstOrDefault(x => x.Type == AuthenticationConstants.DocsUserId);
                if (docsUserIdClaim != null)
                {
                    var userId = ClaimHelper.ExtractUserIdFromClaim(docsUserIdClaim.Value);
                    if (!string.IsNullOrEmpty(userId))
                    {
                        return userId;
                    }
                }
            }

            return null;
        }
    }
}
