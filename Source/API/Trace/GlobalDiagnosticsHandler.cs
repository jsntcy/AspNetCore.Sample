using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;

using AspNetCore.Sample.Common;
using AspNetCore.Sample.Common.ErrorHandling;
using AspNetCore.Sample.Common.Trace;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;

using Newtonsoft.Json;

namespace AspNetCore.Sample.API.Filters
{
    public class GlobalDiagnosticsHandler
    {
        private static readonly Dictionary<Type, HttpStatusCode> ExceptionCodes = new Dictionary<Type, HttpStatusCode>
        {
            { typeof(NotImplementedException), HttpStatusCode.NotImplemented },
            { typeof(AuthenticationException), HttpStatusCode.Unauthorized },
            { typeof(InvalidCredentialException), HttpStatusCode.Unauthorized },
            { typeof(InvalidOperationException), HttpStatusCode.MethodNotAllowed },
            { typeof(NotSupportedException), HttpStatusCode.MethodNotAllowed },
            { typeof(OperationCanceledException), HttpStatusCode.BadRequest },
            { typeof(TimeoutException), HttpStatusCode.RequestTimeout },
            { typeof(BadHttpRequestException), HttpStatusCode.BadRequest }
        };

        private readonly RequestDelegate _next;

        public GlobalDiagnosticsHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                context.Response.OnStarting(
                    state =>
                {
                    var httpContext = (HttpContext)state;
                    if (!httpContext.Response.Headers.ContainsKey(Constant.XmsOperationId))
                    {
                        context.Response.Headers.Add(Constant.XmsOperationId, System.Diagnostics.Activity.Current.RootId);
                    }

                    context.Response.Headers.Add(Constant.XContentTypeOptions, Constant.NoSniff);
                    return Task.CompletedTask;
                }, context);

                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var actualException = exception;
            if (actualException.GetType() == typeof(AggregateException) && actualException.InnerException != null)
            {
                actualException = actualException.InnerException;
            }

            HttpStatusCode statusCode;
            APIError error = null;
            if (actualException is APIException apiException)
            {
                statusCode = (HttpStatusCode)apiException.Error.HttpStatusCode;
                error = apiException.Error;
            }
            else
            {
                if (ExceptionCodes.ContainsKey(actualException.GetType()))
                {
                    statusCode = ExceptionCodes[actualException.GetType()];
                }
                else
                {
                    statusCode = HttpStatusCode.InternalServerError;
                }

                error = Errors.InternalServerError();
            }

            // trace response error and exception
            var result = JsonConvert.SerializeObject(error);
            Logger.TraceError(result);
            Logger.TraceException(actualException);

            context.Response.ContentType = Constant.ContentTypeJson;
            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(result);
        }
    }
}
