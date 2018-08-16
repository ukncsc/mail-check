using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Dmarc.Common.Api.Domain;
using Newtonsoft.Json.Serialization;

namespace Dmarc.Common.Api.Middleware
{
    public class UnhandledExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        private static readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public UnhandledExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILogger<UnhandledExceptionMiddleware> log)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                log.LogError($"{ex.GetType().ToString()}: {ex.Message}:{System.Environment.NewLine}{ex.StackTrace}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            string message = ex.Message;
            int code = StatusCodes.Status500InternalServerError;

            if (ex is MySqlException)
            {
                message = "We are having trouble communicating with our database. Please contact the Mail Check team at mailcheck@digital.ncsc.gov.uk";
            }
            else
            {
                message = "An unknown error occured.";
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = code;

            return context.Response.WriteAsync(JsonConvert.SerializeObject(new ErrorResponse(message), _jsonSettings));
        }
    }
}
