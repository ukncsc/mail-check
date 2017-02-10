using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.Lambda.Core;

namespace Dmarc.AggregateReport.Api.Auth
{
    internal class AggregateReportApiAuth
    {
        public Task<CustomAuthorizerDocument> Authorize(TokenAuthoriserContext tokenAuthoriserContext, ILambdaContext context)
        {
            string apiKey = Environment.GetEnvironmentVariable("ApiKey");
            string apiArn = Environment.GetEnvironmentVariable("ApiArn");

            if (tokenAuthoriserContext.AuthorizationToken == apiKey)
            {
                return Task.FromResult(Create(apiArn));
            }

            throw new UnauthorizedAccessException();
        }

        private CustomAuthorizerDocument Create(string apiArn)
        {
            List<Statement> statement = new List<Statement>
            {
                new Statement("Allow", new List<string> {"execute-api:Invoke"}, apiArn)
            };

            return new CustomAuthorizerDocument("*", new PolicyDocument(statement));
        }
    }    
}

