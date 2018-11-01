using System.Threading;
using System.Threading.Tasks;

using AspNetCore.Sample.Common;
using AspNetCore.Sample.DataAccessor;
using AspNetCore.Sample.DataContract.Entities;
using AspNetCore.Sample.Repository.Interface;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace AspNetCore.Sample.Repository.Cosmos
{
    public class SampleRepository : CosmosAccessor, ISampleRepository
    {
        public SampleRepository(DocumentClient client, string databaseId, string collectionId)
            : base(client, databaseId, collectionId)
        {
        }

        public async Task<SampleEntity> GetSampleAsync(string sampleId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            Guard.ArgumentNotNullOrEmpty(sampleId, nameof(sampleId));

            var options = new RequestOptions
            {
                PartitionKey = new PartitionKey(sampleId)
            };

            var documentUri = UriFactory.CreateDocumentUri(DatabaseId, CollectionId, "document_id");

            // TODO: optimize retry policy.
            // https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly
            var response = await WithRetry(() => Client.ReadDocumentAsync<SampleEntity>(documentUri, options));

            return response?.Document;
        }
    }
}
