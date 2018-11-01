using System.Net;

using AspNetCore.Sample.API.Middlewares;

using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Sample.API.Controllers
{
    [ApiVersion("1.0")]
    public class PingController : Controller
    {
        /// <summary>
        /// This method is used to return an echo message to tell the client if the server is available or not.
        /// </summary>
        /// <returns>Ok</returns>
        [HttpGet("")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [MiddlewareFilter(typeof(NoCachePipeline))]
        public IActionResult Ping()
        {
            return Ok("Pong");
        }
    }
}
