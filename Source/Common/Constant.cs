namespace AspNetCore.Sample.Common
{
    public static class Constant
    {
        public const string AuthCertName = "AuthCertName";
        public const string KeyVaultUri = "KeyVaultUri";
        public const string Separator = "-";

        public const string ContentTypeJson = "application/json";
        public const string XmsOperationId = "X-ms-Operation-Id";
        public const string XContentTypeOptions = "X-Content-Type-Options";
        public const string NoSniff = "nosniff";

        public const string DefaultCORSPolicyName = "DefaultCORSPolicy";

        public const string SwaggerEndpointUrl = "/swagger/v1.0/swagger.json";
        public const string SwaggerDescription = "Sample Service API V1.0";
        public const string SwaggerVersion = "v1.0";
        public const string SwaggerSchemeName = "CookieToken";

        public const int LowestSampleScore = 1;
        public const int HighestSampleScore = 5;

        public const string NoCacheHeader = "no-store, no-cache";
        public const string DefaultCacheHeader = "public, max-age=300";

        public const int DefaultRetryMillisecondsForEtagNotMatch = 10;
        public const int DefaultRetrySecondsForTooManyRequests = 10;
        public const int DefaultMaxRetryForCosmos = 5;

        public const int MaxReturnedItemCount = 10000;
    }
}
