using System;
using System.Collections.Generic;

namespace Dmarc.ForensicReport.Parser.Lambda.Domain
{
    public class ForensicReport
    {
        private readonly List<EmailPart> _emailParts = new List<EmailPart>();

        public ForensicReport(string providerMessageId)
        {
            ProviderMessageId = providerMessageId;
            EmailParts = _emailParts;
        }

        public IReadOnlyList<EmailPart> EmailParts { get; }
        public string ProviderMessageId { get; }

        public void Add(EmailPart emailPart)
        {
            _emailParts.Add(emailPart);
        }

        public override string ToString()
        {
            return $"{Environment.NewLine}{string.Join(Environment.NewLine, EmailParts)}";
        }
    }
}
