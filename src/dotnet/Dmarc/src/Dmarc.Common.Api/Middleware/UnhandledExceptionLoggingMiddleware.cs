using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Dmarc.Common.Api.Middleware
{
    public class UnhandledExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public UnhandledExceptionLoggingMiddleware(RequestDelegate next)
        {
           _next = next;
        }

        public async Task Invoke(HttpContext context, ILogger<UnhandledExceptionLoggingMiddleware> log)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                log.LogError($"Unhandled exception: {ex.Message}:{System.Environment.NewLine}{ex.StackTrace}");
                
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }
    }
}
