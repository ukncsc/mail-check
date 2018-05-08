using System.Collections.Generic;
using System.Linq;
using Dmarc.AggregateReport.Parser.Lambda.Domain;

namespace Dmarc.AggregateReport.Parser.Lambda.Serialisation
{
    public interface ICsvDenormalisedRecordSerialiser
    {
        string Serialise(DenormalisedRecord denormalisedRecord);
    }

    public class CsvDenormalisedRecordSerialiser : ICsvDenormalisedRecordSerialiser
    {

        private const string DelimiterString = ",";
        private const char Quote = '\"';
        private const char Delimiter = ',';
        private const char CarriageReturn = '\r';
        private const char LineFeed = '\n';

        public string Serialise(DenormalisedRecord denormalisedRecord)
        {
            return string.Join(DelimiterString, new List<string>
            {
                denormalisedRecord.OrginalUri ?? string.Empty,
                denormalisedRecord.OrgName ?? string.Empty,
                denormalisedRecord.Email ?? string.Empty,
                denormalisedRecord.ExtraContactInfo ?? string.Empty,
                denormalisedRecord.BeginDate.ToString("dd-MM-yyyy HH:mm:ss") ?? string.Empty,
                denormalisedRecord.EndDate.ToString("dd-MM-yyyy HH:mm:ss") ?? string.Empty,
                denormalisedRecord.Domain ?? string.Empty,
                denormalisedRecord.Adkim?.ToString() ?? string.Empty,
                denormalisedRecord.Aspf?.ToString() ?? string.Empty,
                denormalisedRecord.P.ToString() ?? string.Empty,
                denormalisedRecord.Sp?.ToString() ?? string.Empty,
                denormalisedRecord.Pct?.ToString() ?? string.Empty,
                denormalisedRecord.SourceIp ?? string.Empty,
                denormalisedRecord.Count.ToString() ?? string.Empty,
                denormalisedRecord.Disposition?.ToString() ?? string.Empty,
                denormalisedRecord.Dkim?.ToString() ?? string.Empty,
                denormalisedRecord.Spf?.ToString() ?? string.Empty,
                denormalisedRecord.Reason ?? string.Empty,
                denormalisedRecord.Comment ?? string.Empty,
                denormalisedRecord.EnvelopeTo ?? string.Empty,
                denormalisedRecord.HeaderFrom ?? string.Empty,
                denormalisedRecord.DkimDomain ?? string.Empty,
                denormalisedRecord.DkimResult ?? string.Empty,
                denormalisedRecord.DkimHumanResult ?? string.Empty,
                denormalisedRecord.SpfDomain ?? string.Empty,
                denormalisedRecord.SpfResult ?? string.Empty
            }.Select(EncodeString));
        }

        private string EncodeString(string inputString)
        {
            List<char> outputChars = new List<char>();
            bool needsEncoding = false;
            foreach (var chr in inputString.ToCharArray())
            {
                if (chr == Delimiter || chr == CarriageReturn || chr == LineFeed)
                {
                    needsEncoding = true;
                }
                else if (chr == Quote)
                {
                    outputChars.Add(Quote);
                    needsEncoding = true;
                }
                outputChars.Add(chr);
            }

            return needsEncoding ? "\"" + new string(outputChars.ToArray()) + "\"" : new string(outputChars.ToArray());
        }
    }
}