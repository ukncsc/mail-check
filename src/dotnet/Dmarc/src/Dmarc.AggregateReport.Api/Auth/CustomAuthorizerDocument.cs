using System.Collections.Generic;
using Newtonsoft.Json;

namespace Dmarc.AggregateReport.Api.Auth
{
    //Note this isnt fully compliant with AWS policy "schema"
    //just contains enough for auth to api gateway
    internal class CustomAuthorizerDocument
    {
        public CustomAuthorizerDocument(string principalId, PolicyDocument policyDocument)
        {
            PrincipalId = principalId;
            PolicyDocument = policyDocument;
        }

        [JsonProperty(PropertyName = "principalId")]
        public string PrincipalId { get; }

        [JsonProperty(PropertyName = "policyDocument")]
        public PolicyDocument PolicyDocument { get; }
    }

    public class PolicyDocument
    {
        public PolicyDocument(List<Statement> statement)
        {
            Version = "2012-10-17";
            Statement = statement;
        }
       
        public string Version { get; }
        public List<Statement> Statement { get; }

    }

    public class Statement
    {
        public Statement(string effect, List<string> action, string resource)
        {
            Effect = effect;
            Action = action;
            Resource = resource;
        }

        public string Effect { get;}
        public List<string> Action { get; }
        public string Resource { get; }
    }
}
