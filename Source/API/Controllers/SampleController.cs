using System.Net;
using System.Threading.Tasks;

using AspNetCore.Sample.API.Middlewares;
using AspNetCore.Sample.Common;
using AspNetCore.Sample.DataContract.Models;
using AspNetCore.Sample.Service.Interface;

using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Sample.API.Controllers
{
    [Route("samples")]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class SampleController : BaseController
    {
        private readonly ISampleService _sampleService;

        public SampleController(ISampleService sampleService)
        {
            _sampleService = sampleService;
        }

        /// <summary>
        /// Gets Sample.
        /// </summary>
        /// <param name="id">The id of the sample</param>
        /// <returns>Ok</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GetSampleResponse), (int)HttpStatusCode.OK)]
        [MiddlewareFilter(typeof(CachePipeline))]
        public async Task<IActionResult> GetSampleAsync(string id)
        {
            Guard.ArgumentNotNullOrEmpty(id, nameof(id));

            var response = await _sampleService.GetSampleAsync(id);

            return Ok(response);
        }
    }
}
