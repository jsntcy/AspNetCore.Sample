using Microsoft.AspNetCore.Builder;

namespace AspNetCore.Sample.API.Middlewares
{
    public class CachePipeline
    {
        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<CacheControlMiddleware>();
        }
    }
}
