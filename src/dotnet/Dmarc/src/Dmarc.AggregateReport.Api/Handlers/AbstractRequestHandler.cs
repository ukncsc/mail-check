using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Dmarc.AggregateReport.Api.Messages;
using Dmarc.Common.Logging;
using FluentValidation.Results;
using Newtonsoft.Json;

namespace Dmarc.AggregateReport.Api.Handlers
{
    internal abstract class RequestHandler{}

    internal interface IRequestHandler
    {
        Task<APIGatewayProxyResponse> ProcessRequest(APIGatewayProxyRequest request, ILambdaContext context);
    }

    internal abstract class AbstractRequestHandler<TRequest, TResponse> : RequestHandler, IRequestHandler
        where TResponse : Response
        where TRequest : Request
    {
        private readonly ILogger _log;

        private static readonly IDictionary<string, string> DefaultResponseHeaders = new Dictionary<string, string>
        {
            {"Content-Type", "application/json"},
            {"Access-Control-Allow-Origin", "*"}
        };

        protected AbstractRequestHandler(ILogger log)
        {
            _log = log;
        }

        public async Task<APIGatewayProxyResponse> ProcessRequest(APIGatewayProxyRequest request,
            ILambdaContext context)
        {
            try
            {
                string acceptHeader;
                if (!CanSatisfyAcceptType(request, out acceptHeader))
                {
                    return CreateErrorResponse(HttpStatusCode.NotAcceptable,
                        $"Not acceptable {acceptHeader ?? "<null>"}, supported types are: application/json");
                }

                TRequest internalRequest = await CreateInternalRequestAsync(request);

                ValidationResult validationResult = await ValidateAsync(internalRequest);
                if (!validationResult.IsValid)
                {
                    return CreateValidationErrorResponse(HttpStatusCode.BadRequest, "Invalid input parameters",
                        validationResult.Errors.Select(_ => _.ToString()).ToList());
                }

                bool domainExists = await ResourceExists(internalRequest);
                if (!domainExists)
                {
                    return CreateErrorResponse(HttpStatusCode.NotFound, $"The resource {request.Path} does not exist.");
                }

                TResponse internalResponse = await CreateInternalResponseAsync(internalRequest);

                return CreateResponse(HttpStatusCode.OK, internalResponse);
            }
            catch (Exception e)
            {
                _log.Error($"The following error occurred processing request {e.Message} {Environment.NewLine} {e.StackTrace}");
                return CreateErrorResponse(HttpStatusCode.InternalServerError,
                    "A problem occured processing the request");
            }
        }

        protected abstract Task<ValidationResult> ValidateAsync(TRequest request);

        protected abstract Task<TRequest> CreateInternalRequestAsync(APIGatewayProxyRequest request);

        protected abstract Task<TResponse> CreateInternalResponseAsync(TRequest request);

        protected virtual Task<bool> ResourceExists(TRequest request)
        {
            return Task.FromResult(true);
        }

        protected virtual bool CanSatisfyAcceptType(APIGatewayProxyRequest request, out string accept)
        {
            accept = null;
            return request.Headers != null &&
                   request.Headers.TryGetValue("Accept", out accept) &&
                   accept.ToLower().Contains("application/json");
        }

        protected APIGatewayProxyResponse CreateResponse(HttpStatusCode statusCode, Response response,
            IDictionary<string, string> headers = null)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int) statusCode,
                Body = JsonConvert.SerializeObject(response),
                Headers = headers ?? DefaultResponseHeaders
            };
        }

        protected APIGatewayProxyResponse CreateErrorResponse(HttpStatusCode statusCode, string message,
            IDictionary<string, string> headers = null)
        {
            ErrorResponse errorResponse = new ErrorResponse(message);
            return CreateResponse(statusCode, errorResponse, headers);
        }

        protected APIGatewayProxyResponse CreateValidationErrorResponse(HttpStatusCode statusCode, string errorMessage,
            List<string> validationErrors, IDictionary<string, string> headers = null)
        {
            ValidataionErrorResponse validataionErrorResponse = new ValidataionErrorResponse(errorMessage,
                validationErrors);
            return CreateResponse(statusCode, validataionErrorResponse, headers);
        }
    }
}