using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Sample.DataContract.Entities;

namespace AspNetCore.Sample.Repository.Interface
{
    public interface ISampleRepository
    {
        Task<SampleEntity> GetSampleAsync(string sampleId, CancellationToken token = default(CancellationToken));
    }
}
