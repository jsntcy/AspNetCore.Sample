using Newtonsoft.Json;

namespace AspNetCore.Sample.DataContract.Models
{
    public class GetSampleResponse
    {
        [JsonProperty("id")]
        public string SampleId { get; set; }
    }
}
