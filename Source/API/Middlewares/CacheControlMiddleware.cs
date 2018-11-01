using System.Linq;
using System.Threading.Tasks;

using AspNetCore.Sample.Common;

using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace AspNetCore.Sample.API.Middlewares
{
    public class CacheControlMiddleware
    {
        private readonly RequestDelegate _next;

        public CacheControlMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.OnStarting(
                state =>
                {
                    IHeaderDictionary responseHeaders = context.Response.Headers;
                    if (context.Response.StatusCode == StatusCodes.Status200OK)
                    {
                        responseHeaders[HeaderNames.CacheControl] = Constant.DefaultCacheHeader;
                    }

                    if (!responseHeaders[HeaderNames.CacheControl].Any())
                    {
                        responseHeaders[HeaderNames.CacheControl] = Constant.NoCacheHeader;
                    }

                    return Task.CompletedTask;
                }, context);

            await _next(context).ConfigureAwait(false);
        }
    }
}
