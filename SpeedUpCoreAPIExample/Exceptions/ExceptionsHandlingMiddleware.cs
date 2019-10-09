using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace SpeedUpCoreAPIExample.Exceptions
{
    public class ExceptionsHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionsHandlingMiddleware> _logger;

        public ExceptionsHandlingMiddleware(RequestDelegate next, ILogger<ExceptionsHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleUnhandledExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleUnhandledExceptionAsync(HttpContext context,
                                Exception exception)
        {
            _logger.LogError(exception, exception.Message);

            if (!context.Response.HasStarted)
            {
                int statusCode = (int)HttpStatusCode.InternalServerError; // 500
                string message = string.Empty;
#if DEBUG
                message = exception.Message;
#else
                message = "An unhandled exception has occurred";
#endif
                context.Response.Clear();
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;

                var result = new ExceptionMessage(message).ToString();
                await context.Response.WriteAsync(result);
            }
        }
    }
}