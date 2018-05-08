using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dmarc.MxSecurityTester.Util;

namespace Dmarc.MxSecurityTester.Smtp
{
    public interface ISmtpDeserializer
    {
        Task<SmtpResponse> Deserialize(IStreamReader reader);
    }

    public class SmtpDeserializer : ISmtpDeserializer
    {
        private readonly Regex _regex = new Regex("^[0-9]{3}-");
        private const char Space = ' ';
        private const char Hyphen = '-';

        public async Task<SmtpResponse> Deserialize(IStreamReader reader)
        {
            List<string> values = new List<string>();
            string response;
            do
            {
                response = await reader.ReadLineAsync();
                if (response != null)
                {
                    values.Add(response);
                }
            } while (response != null && _regex.IsMatch(values.Last()));

            List<Response> responses = values.Select(Parse).ToList();

            return new SmtpResponse(responses);
        }

        private Response Parse(string response)
        {
            if (response.Length < 5)
            {
                throw new ArgumentException($"SMTP response ({response}) must be at 5 characters long.");
            }

            string responseCodeString = response.Substring(0, 3);

            int intResponseCode;
            if (!int.TryParse(responseCodeString, out intResponseCode))
            {
                throw new ArgumentException($"SMTP response code ({responseCodeString}) from response ({response}) must be numeric.");
            }

            ResponseCode responseCode = Enum.IsDefined(typeof(ResponseCode), intResponseCode)
                ? (ResponseCode) intResponseCode
                : ResponseCode.Unknown; 

            char separator = response[3];

            if (separator != Space && separator != Hyphen)
            {
                throw new ArgumentException($"SMTP repsonse separator {separator} is not space or hypen.");
            }

            string value = response.Substring(4, response.Length - 4);

            return new Response(responseCode, value, response);
        }
    }
}