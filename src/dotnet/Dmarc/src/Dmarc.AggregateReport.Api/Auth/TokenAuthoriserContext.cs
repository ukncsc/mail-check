namespace Dmarc.AggregateReport.Api.Auth
{
    internal class TokenAuthoriserContext
    {
        public TokenAuthoriserContext(string type, string authorizationToken, string methodArn)
        {
            Type = type;
            AuthorizationToken = authorizationToken;
            MethodArn = methodArn;
        }

        public string Type { get; set; }
        public string AuthorizationToken { get; set; }
        public string MethodArn { get; set; }

        public override string ToString()
        {
            return $"{nameof(Type)}: {Type}, {nameof(AuthorizationToken)}: {AuthorizationToken}, {nameof(MethodArn)}: {MethodArn}";
        }
    }
}