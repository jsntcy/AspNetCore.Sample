using System.Threading;
using System.Threading.Tasks;

using AspNetCore.Sample.Common;
using AspNetCore.Sample.DataContract.Models;
using AspNetCore.Sample.Repository.Interface;
using AspNetCore.Sample.Service.Interface;
using AutoMapper;

namespace AspNetCore.Sample.Service.Implementation
{
    public class SampleService : ISampleService
    {
        private readonly ISampleRepository _sampleRepository;

        public SampleService(ISampleRepository sampleRepository)
        {
            _sampleRepository = sampleRepository;
        }

        public async Task<GetSampleResponse> GetSampleAsync(string id, CancellationToken token)
        {
            Guard.ArgumentNotNullOrEmpty(id, nameof(id));

            var sampleEntity = await _sampleRepository.GetSampleAsync(id, token);
            if (sampleEntity == null)
            {
                return new GetSampleResponse
                {
                    SampleId = id
                };
            }

            return Mapper.Map<GetSampleResponse>(sampleEntity);
        }
    }
}
