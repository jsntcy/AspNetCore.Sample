using System.Threading;
using System.Threading.Tasks;

using AspNetCore.Sample.DataContract.Models;

namespace AspNetCore.Sample.Service.Interface
{
    public interface ISampleService
    {
        Task<GetSampleResponse> GetSampleAsync(string sampleId, CancellationToken token = default(CancellationToken));
    }
}
