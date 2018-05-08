using Dmarc.DnsRecord.Evaluator.Dmarc.Domain;
using Dmarc.DnsRecord.Evaluator.Rules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dmarc.DnsRecord.Evaluator.Dmarc.Rules.Record
{
    public abstract class TagsShouldContainDmarcServiceMailBox<T> : IRule<DmarcRecord>
        where T : ReportUri
    {
        private readonly string _shouldContainDmarcServiceMailBoxErrorMessageFormatString;
        private readonly string _shouldNotHaveMisconfiguredMailCheckMailboxFormatString;
        private readonly string _shouldNotContainDuplicateUris;
        private readonly Uri _dmarcMailbox;
        private string _dmarcMailboxAddress;

        protected TagsShouldContainDmarcServiceMailBox(
            string shouldContainDmarcServiceMailBoxErrorMessageFormatString,
            string shouldNotHaveMisconfiguredMailCheckMailboxFormatString,
            string shouldNotContainDuplicateUris,
            Uri dmarcMailbox)
        {
            _shouldContainDmarcServiceMailBoxErrorMessageFormatString = shouldContainDmarcServiceMailBoxErrorMessageFormatString;
            _shouldNotHaveMisconfiguredMailCheckMailboxFormatString = shouldNotHaveMisconfiguredMailCheckMailboxFormatString;
            _shouldNotContainDuplicateUris = shouldNotContainDuplicateUris;
            _dmarcMailbox = dmarcMailbox;
            _dmarcMailboxAddress = $"{_dmarcMailbox.UserInfo}@{_dmarcMailbox.Host}";
        }

        public bool IsErrored(DmarcRecord record, out Error error)
        {
            var reportUris = record.Tags.OfType<T>().ToList();

            // If we have duplicate entries for the same tag
            // There is already an error so disable this rule
            if (reportUris.Count > 1)
            {
                error = null;
                return false;
            }

            var mailCheckUris = GetMailCheckUris(reportUris.FirstOrDefault());

            if (HasMisconfiguredUri(mailCheckUris))
            {
                error = new Error(ErrorType.Error, string.Format(
                    _shouldNotHaveMisconfiguredMailCheckMailboxFormatString,
                    _dmarcMailboxAddress,
                    _dmarcMailbox.OriginalString));

                return true;
            }

            if (!mailCheckUris.Any())
            {
                error = new Error(ErrorType.Warning, string.Format(
                    _shouldContainDmarcServiceMailBoxErrorMessageFormatString,
                    _dmarcMailboxAddress,
                    _dmarcMailbox.OriginalString));

                return true;
            }

            if (HasDuplicates(reportUris.FirstOrDefault()))
            {
                error = new Error(ErrorType.Warning, _shouldNotContainDuplicateUris);
                return true;
            }

            error = null;
            return false;
        }

        private List<Uri> GetMailCheckUris(T reportUri)
        {
            return reportUri?.Uris?
                .Select(_ => _.Uri.Uri)
                .Where(_ => _?.Authority == _dmarcMailbox.Authority)
                .ToList() ?? new List<Uri>();
        }

        private bool HasMisconfiguredUri(IEnumerable<Uri> uris)
        {
            return uris.Any(_ => _?.UserInfo != _dmarcMailbox.UserInfo);
        }

        private static bool HasDuplicates(T reportUri)
        {
            return reportUri?.Uris?
                .Select(_ => _.Uri.Uri)
                .Where(_ => _ != null)
                .GroupBy(_ => _.OriginalString)
                .Any(_ => _.Count() > 1) ?? false;
        }
    }
}
