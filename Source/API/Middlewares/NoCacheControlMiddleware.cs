using System.Linq;
using System.Threading.Tasks;

using AspNetCore.Sample.Common;

using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace AspNetCore.Sample.API.Middlewares
{
    public class NoCacheControlMiddleware
    {
        private readonly RequestDelegate _next;

        public NoCacheControlMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.OnStarting(
                state =>
                {
                    IHeaderDictionary responseHeaders = context.Response.Headers;
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