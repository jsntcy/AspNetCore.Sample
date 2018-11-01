using Microsoft.Azure.Documents;

using Newtonsoft.Json;

namespace AspNetCore.Sample.DataContract.Entities
{
    public class SampleEntity : Resource
    {
        [JsonProperty("id")]
        public override string Id => "id_override";

        [JsonProperty("sampleId")]
        public string SampleId { get; set; }
    }
}
