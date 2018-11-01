namespace AspNetCore.Sample.Common.Configurations
{
    public class AppSettings
    {
        public string KeyVaultUri { get; set; }

        public string AuthCertName { get; set; }

        public string CosmosConnectionString { get; set; }

        public string CosmosDatabaseId { get; set; }

        public string CosmosSampleCollectionId { get; set; }

        public string[] AllowedCorsOrigins { get; set; }

        public bool UseRealtimeSampleCalculation { get; set; }

        public int RetryMillisecondsForEtagNotMatch { get; set; } = 10;

        public int RetrySecondsForTooManyRequests { get; set; } = 10;

        public int MaxRetryForCosmos { get; set; } = 5;
    }
}
