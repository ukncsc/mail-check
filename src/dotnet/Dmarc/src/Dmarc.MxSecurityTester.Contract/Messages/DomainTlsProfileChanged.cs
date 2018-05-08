namespace Dmarc.MxSecurityTester.Contract.Messages
{
    public class DomainTlsProfileChanged
    {
        public DomainTlsProfileChanged(int domainId)
        {
            DomainId = domainId;
        }

        public int DomainId { get; }
    }
}
