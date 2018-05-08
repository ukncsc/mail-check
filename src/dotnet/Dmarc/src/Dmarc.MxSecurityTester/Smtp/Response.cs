namespace Dmarc.MxSecurityTester.Smtp
{
    public class Response
    {
        public Response(ResponseCode responseCode, string value, string originalValue)
        {
            ResponseCode = responseCode;
            Value = value;
            OriginalValue = originalValue;
        }

        public ResponseCode ResponseCode { get; }

        public string Value { get; }

        public string OriginalValue { get; }

        public override string ToString()
        {
            return $"{OriginalValue}";
        }
    }
}