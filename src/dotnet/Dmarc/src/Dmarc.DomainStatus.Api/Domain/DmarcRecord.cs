using System;

namespace Dmarc.DomainStatus.Api.Domain
{
    public class DmarcRecord : DnsRecord
    {
        public DmarcRecord(int id, string record, DateTime? mxLastChecked)
            : base(id, mxLastChecked)
        {
            Record = record;
        }

        public string Record { get; }
    }
}