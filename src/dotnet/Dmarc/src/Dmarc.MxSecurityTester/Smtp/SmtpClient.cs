using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.MxSecurityTester.Config;
using Dmarc.MxSecurityTester.Util;

namespace Dmarc.MxSecurityTester.Smtp
{
    internal interface ISmtpClient
    {
        Task<bool> TryStartTls(Stream networkStream);
    }

    internal class SmtpClient : ISmtpClient
    {
        private const string Starttls = "starttls";
        private const string LineEnding = "\r\n";

        private readonly ISmtpSerializer _smtpSerializer;
        private readonly ISmtpDeserializer _smtpDeserializer;
        private readonly IMxSecurityTesterConfig _mxSecurityTesterConfig;

        private readonly ILogger _log;

        public SmtpClient(ISmtpSerializer smtpSerializer,
            ISmtpDeserializer smtpDeserializer,
            IMxSecurityTesterConfig mxSecurityTesterConfig,
            ILogger log)
        {
            _smtpSerializer = smtpSerializer;
            _smtpDeserializer = smtpDeserializer;
            _mxSecurityTesterConfig = mxSecurityTesterConfig;
            _log = log;
        }

        public async Task<bool> TryStartTls(Stream networkStream)
        {
            using (IStreamReader streamReader = new StreamReaderWrapper(networkStream, Encoding.ASCII, true, 1024, true))
            {
                using (IStreamWriter streamWriter = new StreamWriterWrapper(networkStream, Encoding.ASCII, 1024, true) {AutoFlush = true, NewLine = LineEnding})
                {
                    SmtpResponse response1 = await _smtpDeserializer.Deserialize(streamReader);
                    _log.Debug($"<: {response1}");
                    if (response1.Responses.FirstOrDefault()?.ResponseCode != ResponseCode.ServiceReady)
                    {
                        return false;
                    }
                    
                    EhloCommand ehloCommand = new EhloCommand(_mxSecurityTesterConfig.SmtpHostName);
                    _log.Debug($">: {ehloCommand.CommandString}");
                    await _smtpSerializer.Serialize(ehloCommand, streamWriter);
                    SmtpResponse response2 = await _smtpDeserializer.Deserialize(streamReader);
                    _log.Debug($"<: {response2}");
                    if (!response2.Responses.Any(_ => _.Value.ToLower() == Starttls && _.ResponseCode == ResponseCode.Ok))
                    {
                        return false;
                    }

                    StartTlsCommand startTlsCommand = new StartTlsCommand();
                    _log.Debug($">: {startTlsCommand.CommandString}");
                    await _smtpSerializer.Serialize(startTlsCommand, streamWriter);
                    SmtpResponse response3 = await _smtpDeserializer.Deserialize(streamReader);
                    _log.Debug($"<: {response3}");

                    return response3.Responses.FirstOrDefault()?.ResponseCode == ResponseCode.ServiceReady;
                }
            }
        }
    }
}