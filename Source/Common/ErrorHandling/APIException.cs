using System;

namespace AspNetCore.Sample.Common.ErrorHandling
{
    public class APIException : Exception
    {
        public APIError Error { get; set; }

        public object ExtendedInformation { get; set; }

        public APIException(APIError error, Exception innerException = null, object extendedInformation = null)
            : base(error.ErrorMessage, innerException)
        {
            Error = error;
            ExtendedInformation = extendedInformation;
        }
    }
}
