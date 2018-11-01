using Microsoft.AspNetCore.Builder;

namespace AspNetCore.Sample.API.Middlewares
{
    public class NoCachePipeline
    {
        public void Configure(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<NoCacheControlMiddleware>();
        }
    }
}
