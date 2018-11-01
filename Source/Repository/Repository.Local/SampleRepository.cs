using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Sample.Common;
using AspNetCore.Sample.DataContract.Entities;
using AspNetCore.Sample.Repository.Interface;

namespace AspNetCore.Sample.Repository.Local
{
    public class SampleRepository : ISampleRepository
    {
        private readonly Dictionary<string, SampleEntity> _samplesBysampleId;

        public SampleRepository(Dictionary<string, SampleEntity> samplesBysampleId = null)
        {
            _samplesBysampleId = samplesBysampleId ?? new Dictionary<string, SampleEntity>();
        }

        public Task<SampleEntity> GetSampleAsync(string sampleId, CancellationToken token)
        {
            Guard.ArgumentNotNullOrEmpty(sampleId, nameof(sampleId));

            if (!_samplesBysampleId.TryGetValue(sampleId, out var entity))
            {
                entity = new SampleEntity
                {
                    SampleId = sampleId
                };
            }

            return Task.FromResult(entity);
        }
    }
}
