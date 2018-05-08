using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.DnsRecord.Importer.Lambda.Dns;
using Dmarc.DnsRecord.Importer.Lambda.Dns.Client;
using Dmarc.DnsRecord.Importer.Lambda.Dns.Client.RecordInfos;
using FakeItEasy;
using Heijden.DNS;
using NUnit.Framework;

namespace Dmarc.DnsRecord.Importer.Lambda.Test.Client
{
    [TestFixture]
    public class SpfRecordDnsClientTests
    {
        private SpfRecordDnsClient _spfRecordDnsClient;
        private IDnsResolver _dnsResolver;
        private ILogger _logger;

        [SetUp]
        public void SetUp()
        {
            _dnsResolver = A.Fake<IDnsResolver>();
            _logger = A.Fake<ILogger>();
            _spfRecordDnsClient = new SpfRecordDnsClient(_dnsResolver, _logger);
        }

        [Test]
        public async Task NoSpfRecordReturnFromClientReturnsNull()
        {
            string[] records = { "AFASDFA" };
            string domain = "abc.gov.uk";
            Response dnsQueryResponse = CreateRecord(domain, records);

            A.CallTo(() => _dnsResolver.GetRecord(A<string>._, A<QType>._)).Returns(Task.FromResult(dnsQueryResponse));

            DnsResponse spfRecords = await _spfRecordDnsClient.GetRecord("abc.gov.uk");

            Assert.That(spfRecords.Records.Count, Is.EqualTo(1));
            Assert.That(spfRecords.Records[0], Is.EqualTo(SpfRecordInfo.EmptyRecordInfo));
        }

        [Test]
        public async Task SpfReturnedFromClientReturnsSpfRecord()
        {
            string[] records = { "v=spf1 include: abc.xyx.com -all" };
            string domain = "abc.gov.uk";
            Response dnsQueryResponse = CreateRecord(domain, records);

            A.CallTo(() => _dnsResolver.GetRecord(A<string>._, A<QType>._)).Returns(Task.FromResult(dnsQueryResponse));

            DnsResponse spfRecords = await _spfRecordDnsClient.GetRecord("abc.gov.uk");

            Assert.That(spfRecords.Records.Count, Is.EqualTo(1));
            Assert.That(((SpfRecordInfo)spfRecords.Records[0]).Record, Is.EqualTo(records[0]));
        }

        [Test]
        public async Task ErrorReturnFromClientRecordIsErrored()
        {
            string[] records = { "v=spf1 include: abc.xyx.com -all" };
            string domain = "abc.gov.uk";
            Response dnsQueryResponse = CreateRecord(domain, records, RCode.ServFail);

            A.CallTo(() => _dnsResolver.GetRecord(A<string>._, A<QType>._)).Returns(Task.FromResult(dnsQueryResponse));

            DnsResponse spfRecord = await _spfRecordDnsClient.GetRecord("abc.gov.uk");

            Assert.That(spfRecord.ResponseCode, Is.EqualTo(RCode.ServFail));
            A.CallTo(() => _logger.Error(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task IsCaseInsensitive()
        {
            string[] records = { "V=SpF1 include: abc.xyx.com -all" };
            string domain = "abc.gov.uk";
            Response dnsQueryResponse = CreateRecord(domain, records);

            A.CallTo(() => _dnsResolver.GetRecord(A<string>._, A<QType>._)).Returns(Task.FromResult(dnsQueryResponse));

            DnsResponse spfRecords = await _spfRecordDnsClient.GetRecord("abc.gov.uk");

            Assert.That(spfRecords.Records.Count, Is.EqualTo(1));
            Assert.That(((SpfRecordInfo)spfRecords.Records[0]).Record, Is.EqualTo(records[0]));
        }

        [Test]
        public async Task MultipleSpfReturnedFromClientReturnsMultipleSpfRecords()
        {
            string[] records = { "v=spf1 include: abc.xyx.com -all", "v=spf1 include: abc.xyx.com ~all" };
            string domain = "abc.gov.uk";
            Response dnsQueryResponse = CreateRecord(domain, records);

            A.CallTo(() => _dnsResolver.GetRecord(A<string>._, A<QType>._)).Returns(Task.FromResult(dnsQueryResponse));

            DnsResponse spfRecords = await _spfRecordDnsClient.GetRecord("abc.gov.uk");

            Assert.That(spfRecords.Records.Count, Is.EqualTo(2));
            Assert.That(((SpfRecordInfo)spfRecords.Records[0]).Record, Is.EqualTo(records[0]));
            Assert.That(((SpfRecordInfo)spfRecords.Records[1]).Record, Is.EqualTo(records[1]));
        }

        [Test]
        public async Task MultipleRecordPartsSingleRecordCorrectlyExtracted()
        {
            string[] records = { "v=spf1 in", "clude: abc.xyx.com -all" };
            string domain = "abc.gov.uk";
            Response dnsQueryResponse = CreateRecord(domain, records);

            A.CallTo(() => _dnsResolver.GetRecord(A<string>._, A<QType>._)).Returns(Task.FromResult(dnsQueryResponse));

            DnsResponse spfRecords = await _spfRecordDnsClient.GetRecord("abc.gov.uk");

            Assert.That(spfRecords.Records.Count, Is.EqualTo(1));
            Assert.That(((SpfRecordInfo)spfRecords.Records[0]).Record, Is.EqualTo(string.Join(string.Empty, records[0])));
        }

        private Response CreateRecord(string domainName, string[] records, RCode responseCode = RCode.NoError)
        {
            return CreateRecord(domainName, records.Select(_ => new[] {_}).ToArray(), responseCode);
        }

        private Response CreateRecord(string domainName, string[][] records, RCode responseCode = RCode.NoError)
        {
            Response response = new Response { header = { RCODE = responseCode } };
            foreach (var record in records)
            {
                QType dnsEntryType = QType.TXT;
                Class dnsClass = Class.IN;

                byte nameTerminator = 0;

                byte[] domainNameBytes = Encoding.UTF8.GetBytes(domainName);
                byte[] dnsEntryTypeBytes = BitConverter.GetBytes((UInt16)dnsEntryType).Reverse().ToArray();
                byte[] dnsClassBytes = BitConverter.GetBytes((UInt16)dnsClass).Reverse().ToArray();
                byte[] ttlBytes = BitConverter.GetBytes(3600).Reverse().ToArray();
                byte[][] recordsBytes = record.Select(_ => Encoding.UTF8.GetBytes(_)).ToArray();
                byte[] lengthBytes = BitConverter.GetBytes((UInt16)recordsBytes.Sum(_ => _.Length)).Reverse().ToArray();


                using (MemoryStream memoryStream = new MemoryStream())
                {
                    memoryStream.WriteByte((byte)domainNameBytes.Length);
                    memoryStream.Write(domainNameBytes, 0, domainNameBytes.Length);
                    memoryStream.WriteByte(nameTerminator);
                    memoryStream.Write(dnsEntryTypeBytes, 0, dnsEntryTypeBytes.Length);
                    memoryStream.Write(dnsClassBytes, 0, dnsClassBytes.Length);
                    memoryStream.Write(ttlBytes, 0, ttlBytes.Length);
                    memoryStream.Write(lengthBytes, 0, lengthBytes.Length);

                    foreach (var bytes in recordsBytes)
                    {
                        memoryStream.WriteByte((byte)bytes.Length);
                        memoryStream.Write(bytes, 0, bytes.Length);
                    }

                    response.Answers.Add(new AnswerRR(new RecordReader(memoryStream.ToArray())));
                }
            }
            return response;
        }
    }
}
