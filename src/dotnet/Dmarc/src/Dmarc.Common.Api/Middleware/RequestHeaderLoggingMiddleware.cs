using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Dmarc.Common.Api.Middleware
{
    public class RequestHeaderLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestHeaderLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILogger<RequestHeaderLoggingMiddleware> log)
        {
            string headers = string.Join(System.Environment.NewLine, context.Request.Headers.Select(_ => $"{_.Key}:{_.Value}"));

            log.LogDebug($"Request Headers:{System.Environment.NewLine}{headers}");

            await _next(context);
        }
    }
}