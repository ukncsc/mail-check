using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Dmarc.AggregateReport.Api.Handlers;
using Dmarc.AggregateReport.Api.Messages;
using Dmarc.Common.Logging;
using FakeItEasy;
using FluentValidation.Results;
using NUnit.Framework;

namespace Dmarc.AggregateReport.Api.Test.Handlers
{
    [TestFixture]
    public class AbstractRequestHandlerTests
    {
        private TestRequestHandler _testRequestHandler;

        [SetUp]
        public void SetUp()
        {
            _testRequestHandler = new TestRequestHandler();
        }


        [Test]
        public async Task RequestDoesntContainJsonAcceptHeaderReturnsNotAccptableResponse()
        {
            APIGatewayProxyResponse gatewayProxyResponse = await _testRequestHandler.ProcessRequest(new APIGatewayProxyRequest(), A.Fake<ILambdaContext>());

            Assert.That(gatewayProxyResponse.StatusCode, Is.EqualTo((int)HttpStatusCode.NotAcceptable));
        }

        [Test]
        public async Task RequestContainsJsonAcceptHeaderReturnsOkResponse()
        {
            APIGatewayProxyRequest apiGatewayProxyRequest = new APIGatewayProxyRequest
            {
                Headers = new Dictionary<string, string> { {"Accept", "application/json"} }
            };

            APIGatewayProxyResponse gatewayProxyResponse = await _testRequestHandler.ProcessRequest(apiGatewayProxyRequest, A.Fake<ILambdaContext>());

            Assert.That(gatewayProxyResponse.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task InternalRequestIsInvalidReturnsBadRequestResponse()
        {
            APIGatewayProxyRequest apiGatewayProxyRequest = new APIGatewayProxyRequest
            {
                Headers = new Dictionary<string, string> { { "Accept", "application/json" } }
            };

            _testRequestHandler.ValidationResult = new ValidationResult(new List<ValidationFailure> {new ValidationFailure("property","error")});

            APIGatewayProxyResponse gatewayProxyResponse = await _testRequestHandler.ProcessRequest(apiGatewayProxyRequest, A.Fake<ILambdaContext>());

            Assert.That(gatewayProxyResponse.StatusCode, Is.EqualTo((int)HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task InternalRequestIsValidReturnsOkResponse()
        {
            APIGatewayProxyRequest apiGatewayProxyRequest = new APIGatewayProxyRequest
            {
                Headers = new Dictionary<string, string> { { "Accept", "application/json" } }
            };

            APIGatewayProxyResponse gatewayProxyResponse = await _testRequestHandler.ProcessRequest(apiGatewayProxyRequest, A.Fake<ILambdaContext>());

            Assert.That(gatewayProxyResponse.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }

        [Test]
        public async Task ResourceDoesntExistReturnsNotFoundResponse()
        {
            APIGatewayProxyRequest apiGatewayProxyRequest = new APIGatewayProxyRequest
            {
                Headers = new Dictionary<string, string> { { "Accept", "application/json" } }
            };

            _testRequestHandler.ResourceExistsProp = false;

            APIGatewayProxyResponse gatewayProxyResponse = await _testRequestHandler.ProcessRequest(apiGatewayProxyRequest, A.Fake<ILambdaContext>());

            Assert.That(gatewayProxyResponse.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }

        [Test]
        public async Task ResourseDoesExistReturnsOkResponse()
        {
            APIGatewayProxyRequest apiGatewayProxyRequest = new APIGatewayProxyRequest
            {
                Headers = new Dictionary<string, string> { { "Accept", "application/json" } }
            };

            APIGatewayProxyResponse gatewayProxyResponse = await _testRequestHandler.ProcessRequest(apiGatewayProxyRequest, A.Fake<ILambdaContext>());

            Assert.That(gatewayProxyResponse.StatusCode, Is.EqualTo((int)HttpStatusCode.OK));
        }


        private class TestRequestHandler : AbstractRequestHandler<DateRangeDomainRequest, Response>
        {
            public TestRequestHandler()
                : base(A.Fake<ILogger>())
            {
                ValidationResult = new ValidationResult(new List<ValidationFailure>());
                InternalRequest = new DateRangeDomainRequest(DateTime.Now, DateTime.Now, null);
                Response = new AggregatedStatisticsResponse(new Dictionary<string, int> {{"test",1 }});
                ResourceExistsProp = true;
            }

            public ValidationResult ValidationResult { get; set; }

            public DateRangeDomainRequest InternalRequest { get; set; }

            public AggregatedStatisticsResponse Response { get; set; }

            public bool ResourceExistsProp { get; set; }

            protected override Task<ValidationResult> ValidateAsync(DateRangeDomainRequest request)
            {
                return Task.FromResult(ValidationResult);
            }

            protected override Task<DateRangeDomainRequest> CreateInternalRequestAsync(APIGatewayProxyRequest request)
            {
                return Task.FromResult(InternalRequest);
            }

            protected override Task<Response> CreateInternalResponseAsync(DateRangeDomainRequest request)
            {
                return Task.FromResult((Response)Response);
            }

            protected override Task<bool> ResourceExists(DateRangeDomainRequest request)
            {
                return Task.FromResult(ResourceExistsProp);
            }
        }
    }
}
