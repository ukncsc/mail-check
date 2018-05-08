using System;
using System.Collections.Immutable;
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
    public class MxRecordDnsClientTest
    {
        private IDnsResolver _dnsResolver;
        private ILogger _logger;
        private MxRecordDnsClient _mxRecordDnsClient;

        [SetUp]
        public void SetUp()
        {
            _dnsResolver = A.Fake<IDnsResolver>();
            _logger = A.Fake<ILogger>();
            _mxRecordDnsClient = new MxRecordDnsClient(_dnsResolver, _logger);
        }

        [Test]
        public async Task NoMxRecordReturnFromClientReturnsNull()
        {
            string[] records = new string[0];
            string domain = "abc.gov.uk";
            Response dnsQueryResponse = CreateRecord(domain, records);

            A.CallTo(() => _dnsResolver.GetRecord(A<string>._, A<QType>._)).Returns(Task.FromResult(dnsQueryResponse));

            DnsResponse mxRecords = await _mxRecordDnsClient.GetRecord(domain);

            Assert.That(mxRecords.Records.Count, Is.EqualTo(1));
            Assert.That(mxRecords.Records[0], Is.EqualTo(MxRecordInfo.EmptyRecordInfo));
        }

        [Test]
        public async Task MxRecordReturnedFromClientReturnsMxRecord()
        {
            string[] records = { "a.b.com" };
            string domain = "abc.gov.uk";
            Response dnsQueryResponse = CreateRecord(domain, records);

            A.CallTo(() => _dnsResolver.GetRecord(A<string>._, A<QType>._)).Returns(Task.FromResult(dnsQueryResponse));

            DnsResponse mxRecords = await _mxRecordDnsClient.GetRecord(domain);

            Assert.That(mxRecords.Records.Count, Is.EqualTo(1));
            Assert.That(((MxRecordInfo)mxRecords.Records[0]).Host, Is.EqualTo($"{records[0]}."));
            Assert.That(((MxRecordInfo)mxRecords.Records[0]).Preference, Is.EqualTo(0));
        }

        [Test]
        public async Task ErrorReturnFromClientRecordIsErrored()
        {
            string[] records = { "a.b.com" };
            string domain = "abc.gov.uk";
            Response dnsQueryResponse = CreateRecord(domain, records, true);

            A.CallTo(() => _dnsResolver.GetRecord(A<string>._, A<QType>._)).Returns(Task.FromResult(dnsQueryResponse));

            DnsResponse mxRecords = await _mxRecordDnsClient.GetRecord(domain);

            Assert.That(mxRecords.ResponseCode, Is.EqualTo(RCode.ServFail));
            A.CallTo(() => _logger.Error(A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task MultipleMxReturnedFromClientReturnsMultipleMxRecords()
        {
            string[] records = { "a.b1.com", "a.b2.com" };
            string domain = "abc.gov.uk";
            Response dnsQueryResponse = CreateRecord(domain, records);

            A.CallTo(() => _dnsResolver.GetRecord(A<string>._, A<QType>._)).Returns(Task.FromResult(dnsQueryResponse));

            DnsResponse mxRecords = await _mxRecordDnsClient.GetRecord(domain);

            Assert.That(mxRecords.Records.Count, Is.EqualTo(2));
            Assert.That(((MxRecordInfo)mxRecords.Records[0]).Host, Is.EqualTo($"{records[0]}."));
            Assert.That(((MxRecordInfo)mxRecords.Records[0]).Preference, Is.EqualTo(0));
            Assert.That(((MxRecordInfo)mxRecords.Records[1]).Host, Is.EqualTo($"{records[1]}."));
            Assert.That(((MxRecordInfo)mxRecords.Records[1]).Preference, Is.EqualTo(1));
        }

        private Response CreateRecord(string domainName, string[] records, bool hasError = false)
        {
            QType dnsEntryType = QType.MX;
            Class dnsClass = Class.IN;
            UInt32 ttl = 3600;

            byte nameTerminator = 0;

            byte[] domainNameBytes = Encoding.UTF8.GetBytes(domainName);
            byte[] dnsEntryTypeBytes = BitConverter.GetBytes((UInt16)dnsEntryType).Reverse().ToArray();
            byte[] dnsClassBytes = BitConverter.GetBytes((UInt16)dnsClass).Reverse().ToArray();
            byte[] ttlBytes = BitConverter.GetBytes(ttl).Reverse().ToArray();
            byte[] lengthBytes = BitConverter.GetBytes((UInt16)0).Reverse().ToArray();
            

            Response response = new Response();
            if (hasError)
            {
                response.header.RCODE = RCode.ServFail;
            }

            for (int i = 0; i < records.Length; i++)
            {
                byte[] preferenceBytes = BitConverter.GetBytes((UInt16)i).Reverse().ToArray();
                byte[] recordBytes = Encoding.UTF8.GetBytes(records[i]);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    memoryStream.WriteByte((byte)domainNameBytes.Length);
                    memoryStream.Write(domainNameBytes, 0, domainNameBytes.Length);
                    memoryStream.WriteByte(nameTerminator);
                    memoryStream.Write(dnsEntryTypeBytes, 0, dnsEntryTypeBytes.Length);
                    memoryStream.Write(dnsClassBytes, 0, dnsClassBytes.Length);
                    memoryStream.Write(ttlBytes, 0, ttlBytes.Length);
                    memoryStream.Write(lengthBytes, 0, lengthBytes.Length);
                    memoryStream.Write(preferenceBytes, 0, preferenceBytes.Length);
                    memoryStream.WriteByte((byte)recordBytes.Length);
                    memoryStream.Write(recordBytes, 0, recordBytes.Length);
                    memoryStream.WriteByte(nameTerminator);

                    response.Answers.Add(new AnswerRR(new RecordReader(memoryStream.ToArray())));
                }
            }

            return response;
        }
    }
}