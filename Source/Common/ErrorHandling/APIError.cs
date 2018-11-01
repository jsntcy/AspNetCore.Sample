using System;

namespace AspNetCore.Sample.Common.ErrorHandling
{
    public class APIError
    {
        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public bool Retriable { get; set; }

        public int HttpStatusCode { get; set; }

        public APIException Exception(Exception innerException = null, object extendedInformation = null)
        {
            return new APIException(this, innerException, extendedInformation);
        }
    }
}
