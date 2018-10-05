using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Interface.PublicSuffix;
using Dmarc.Common.Interface.PublicSuffix.Domain;
using Dmarc.DnsRecord.Importer.Lambda.Dns;
using Dmarc.DnsRecord.Importer.Lambda.Dns.Client;
using Dmarc.DnsRecord.Importer.Lambda.Dns.Client.RecordInfos;
using FakeItEasy;
using Heijden.DNS;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Importer.Lambda.Test.Client
{
    [TestFixture]
    public class DmarcRecordDnsClientTests
    {
        private DmarcRecordDnsClient _dmarcRecordDnsClient;
        private IDnsResolver _dnsResolver;
        private IOrganisationalDomainProvider _organisationalDomainProvider;
        private ILogger _logger;

        [SetUp]
        public void SetUp()
        {
            _dnsResolver = A.Fake<IDnsResolver>();
            _logger = A.Fake<ILogger>();
            _organisationalDomainProvider = A.Fake<IOrganisationalDomainProvider>();
            _dmarcRecordDnsClient = new DmarcRecordDnsClient(_dnsResolver, _logger, _organisationalDomainProvider);
        }

        [Test]
        public async Task NoDmarcRecordReturnFromClientReturnsNull()
        {
            string[] records = {"AFASDFA"};
            string domain = "abc.gov.uk";
            Response dnsQueryResponse = CreateRecord(domain, records);

            A.CallTo(() => _dnsResolver.GetRecord(A<string>._, A<QType>._)).Returns(Task.FromResult(dnsQueryResponse));

            DnsResponse dmarcRecords = await _dmarcRecordDnsClient.GetRecord(domain);

            Assert.That(dmarcRecords.Records.Count, Is.EqualTo(1));
            Assert.That(dmarcRecords.Records[0], Is.EqualTo(DmarcRecordInfo.EmptyRecordInfo));
        }

        [Test]
        public async Task DmarcReturnedFromClientReturnsDmarcRecord()
        {
            string[] records = {"v=DMARC1;p=reject;adkim=s;aspf=s ..."};
            string domain = "abc.gov.uk";
            Response dnsQueryResponse = CreateRecord(domain, records);

            A.CallTo(() => _dnsResolver.GetRecord(A<string>._, A<QType>._)).Returns(Task.FromResult(dnsQueryResponse));

            DnsResponse dmarcRecords = await _dmarcRecordDnsClient.GetRecord(domain);

            Assert.That(dmarcRecords.Records.Count, Is.EqualTo(1));
            Assert.That(((DmarcRecordInfo) dmarcRecords.Records[0]).Record, Is.EqualTo(records[0]));
            A.CallTo(() => _organisationalDomainProvider.GetOrganisationalDomain(A<string>._)).WithAnyArguments()
                .MustHaveHappened();
        }

        [Test]
        public async Task ErrorReturnFromClientRecordIsErrored()
        {
            string[] records = {"v=DMARC1;p=reject;adkim=s;aspf=s ..."};
            string domain = "abc.gov.uk";
            Response dnsQueryResponse = CreateRecord(domain, records, RCode.ServFail);

            A.CallTo(() => _dnsResolver.GetRecord(A<string>._, A<QType>._)).Returns(Task.FromResult(dnsQueryResponse));

            DnsResponse dmarcRecords = await _dmarcRecordDnsClient.GetRecord("abc.gov.uk");

            Assert.That(dmarcRecords.ResponseCode, Is.EqualTo(RCode.ServFail));
            A.CallTo(() => _logger.Error(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _organisationalDomainProvider.GetOrganisationalDomain(A<string>._)).WithAnyArguments()
                .MustHaveHappened();
        }

        [Test]
        public async Task IsCaseInsensitive()
        {
            string[] records = {"v=DmArC1;p=reject;adkim=s;aspf=s ..."};
            string domain = "abc.gov.uk";
            Response dnsQueryResponse = CreateRecord(domain, records);

            A.CallTo(() => _dnsResolver.GetRecord(A<string>._, A<QType>._)).Returns(Task.FromResult(dnsQueryResponse));

            DnsResponse dmarcRecords = await _dmarcRecordDnsClient.GetRecord("abc.gov.uk");

            Assert.That(((DmarcRecordInfo) dmarcRecords.Records[0]).Record, Is.EqualTo(records[0]));
            A.CallTo(() => _organisationalDomainProvider.GetOrganisationalDomain(A<string>._)).WithAnyArguments()
                .MustHaveHappened();
        }

        [Test]
        public async Task MultipleDmarcReturnedFromClientReturnsMultipleDmarcRecords()
        {
            string[] records = {"v=DmArC1;p=reject;adkim=s;aspf=s ...", "v=DmArC1;p=reject;adkim=s;aspf=s ..."};
            string domain = "abc.gov.uk";
            Response dnsQueryResponse = CreateRecord(domain, records);

            A.CallTo(() => _dnsResolver.GetRecord(A<string>._, A<QType>._)).Returns(Task.FromResult(dnsQueryResponse));

            DnsResponse dmarcRecords = await _dmarcRecordDnsClient.GetRecord(domain);

            Assert.That(dmarcRecords.Records.Count, Is.EqualTo(2));
            Assert.That(((DmarcRecordInfo) dmarcRecords.Records[0]).Record, Is.EqualTo(records[0]));
            Assert.That(((DmarcRecordInfo) dmarcRecords.Records[1]).Record, Is.EqualTo(records[1]));
            A.CallTo(() => _organisationalDomainProvider.GetOrganisationalDomain(A<string>._)).WithAnyArguments()
                .MustHaveHappened();
        }

        [Test]
        public async Task MultipleRecordPartsSingleRecordCorrectlyExtracted()
        {
            string[][] records = {new[] {"v=DmArC1;p=reject;adkim=s;as", "pf=r"}};
            string domain = "abc.gov.uk";
            Response dnsQueryResponse = CreateRecord(domain, records);

            A.CallTo(() => _dnsResolver.GetRecord(A<string>._, A<QType>._)).Returns(Task.FromResult(dnsQueryResponse));

            DnsResponse dmarcRecords = await _dmarcRecordDnsClient.GetRecord(domain);

            Assert.That(dmarcRecords.Records.Count, Is.EqualTo(1));
            Assert.That(((DmarcRecordInfo) dmarcRecords.Records[0]).Record,
                Is.EqualTo(string.Join(string.Empty, records[0])));
            A.CallTo(() => _organisationalDomainProvider.GetOrganisationalDomain(A<string>._)).WithAnyArguments()
                .MustHaveHappened();
        }

        [Test]
        public async Task DmarcReturnedFromClientReturnsOrgDmarcRecordWhenDomainDoesNotExist()
        {
            string[] records = {"v=DMARC1;p=reject;adkim=s;aspf=s ..."};
            string orgDomain = "def.gov.uk";
            string domain = "abc.def.gov.uk";

            Response domainDnsQueryResponse = A.Fake<Response>();
            Response orgDomainDnsQueryResponse = CreateRecord(domain, records);
            OrganisationalDomain organisationalDomain = new OrganisationalDomain(orgDomain, domain);

            A.CallTo(() => _dnsResolver.GetRecord($"_dmarc.{domain}", A<QType>._))
                .Returns(Task.FromResult(domainDnsQueryResponse));
            A.CallTo(() => _dnsResolver.GetRecord($"_dmarc.{orgDomain}", A<QType>._))
                .Returns(Task.FromResult(orgDomainDnsQueryResponse));
            A.CallTo(() => _organisationalDomainProvider.GetOrganisationalDomain(A<string>._))
                .Returns(Task.FromResult(organisationalDomain));

            DnsResponse dmarcRecords = await _dmarcRecordDnsClient.GetRecord(domain);

            Assert.That(dmarcRecords.Records.Count, Is.EqualTo(1));
            Assert.That(((DmarcRecordInfo) dmarcRecords.Records[0]).Record, Is.EqualTo(records[0]));
            A.CallTo(() => _organisationalDomainProvider.GetOrganisationalDomain(A<string>._)).WithAnyArguments()
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _dnsResolver.GetRecord(A<string>._, A<QType>._)).WithAnyArguments()
                .MustHaveHappenedTwiceExactly();
        }

        [Test]
        public async Task DoesNotAccessOrgDmarcRecordWhenOrgDomain()
        {
            string[] records = {"v=DMARC1;p=reject;adkim=s;aspf=s ..."};
            string orgDomain = "def.gov.uk";
            string domain = "abc.def.gov.uk";

            Response domainDnsQueryResponse = CreateRecord(domain, records, RCode.NXDomain);
            Response orgDomainDnsQueryResponse = CreateRecord(domain, records);
            OrganisationalDomain organisationalDomain = new OrganisationalDomain(orgDomain, orgDomain);

            A.CallTo(() => _dnsResolver.GetRecord($"_dmarc.{domain}", A<QType>._))
                .Returns(Task.FromResult(domainDnsQueryResponse));
            A.CallTo(() => _dnsResolver.GetRecord($"_dmarc.{orgDomain}", A<QType>._))
                .Returns(Task.FromResult(orgDomainDnsQueryResponse));
            A.CallTo(() => _organisationalDomainProvider.GetOrganisationalDomain(A<string>._))
                .Returns(Task.FromResult(organisationalDomain));

            DnsResponse dmarcRecords = await _dmarcRecordDnsClient.GetRecord(domain);

            Assert.That(dmarcRecords.Records.Count, Is.EqualTo(1));
            Assert.That(((DmarcRecordInfo) dmarcRecords.Records[0]).Record, Is.EqualTo(records[0]));
            A.CallTo(() => _organisationalDomainProvider.GetOrganisationalDomain(A<string>._)).WithAnyArguments()
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => _dnsResolver.GetRecord(A<string>._, A<QType>._)).WithAnyArguments()
                .MustHaveHappenedOnceExactly();
        }

        private Response CreateRecord(string domainName, string[] records, RCode responseCode = RCode.NoError)
        {
            return CreateRecord(domainName, records.Select(_ => new[] {_}).ToArray(), responseCode);
        }

        private Response CreateRecord(string domainName, string[][] records, RCode responseCode = RCode.NoError)
        {
            Response response = new Response {header = {RCODE = responseCode}};
            foreach (var record in records)
            {
                QType dnsEntryType = QType.TXT;
                Class dnsClass = Class.IN;

                byte nameTerminator = 0;

                byte[] domainNameBytes = Encoding.UTF8.GetBytes(domainName);
                byte[] dnsEntryTypeBytes = BitConverter.GetBytes((UInt16) dnsEntryType).Reverse().ToArray();
                byte[] dnsClassBytes = BitConverter.GetBytes((UInt16) dnsClass).Reverse().ToArray();
                byte[] ttlBytes = BitConverter.GetBytes(3600).Reverse().ToArray();
                byte[][] recordsBytes = record.Select(_ => Encoding.UTF8.GetBytes(_)).ToArray();
                byte[] lengthBytes =
                    BitConverter.GetBytes((UInt16) recordsBytes.Sum(_ => _.Length)).Reverse().ToArray();


                using (MemoryStream memoryStream = new MemoryStream())
                {
                    memoryStream.WriteByte((byte) domainNameBytes.Length);
                    memoryStream.Write(domainNameBytes, 0, domainNameBytes.Length);
                    memoryStream.WriteByte(nameTerminator);
                    memoryStream.Write(dnsEntryTypeBytes, 0, dnsEntryTypeBytes.Length);
                    memoryStream.Write(dnsClassBytes, 0, dnsClassBytes.Length);
                    memoryStream.Write(ttlBytes, 0, ttlBytes.Length);
                    memoryStream.Write(lengthBytes, 0, lengthBytes.Length);

                    foreach (var bytes in recordsBytes)
                    {
                        memoryStream.WriteByte((byte) bytes.Length);
                        memoryStream.Write(bytes, 0, bytes.Length);
                    }

                    response.Answers.Add(new AnswerRR(new RecordReader(memoryStream.ToArray())));
                }
            }

            return response;
        }
    }
}