using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dmarc.Common.Interface.Logging;
using Dmarc.Common.Logging;
using Dmarc.Common.Tls.BouncyCastle;
using Dmarc.MxSecurityTester.Config;
using Dmarc.MxSecurityTester.Util;

namespace Dmarc.MxSecurityTester.Smtp
{
    internal interface ISmtpClient
    {
        Task<StartTlsResult> TryStartTls(Stream networkStream);
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

        public async Task<StartTlsResult> TryStartTls(Stream networkStream)
        {
            try
            {
                using (IStreamReader streamReader =
                    new StreamReaderWrapper(networkStream, Encoding.ASCII, true, 1024, true))
                {
                    using (IStreamWriter streamWriter =
                        new StreamWriterWrapper(networkStream, Encoding.ASCII, 1024, true)
                        {
                            AutoFlush = true,
                            NewLine = LineEnding
                        })
                    {
                        SmtpResponse response1 = await _smtpDeserializer.Deserialize(streamReader);
                        _log.Debug($"<: {response1}");

                        if (response1.Responses.FirstOrDefault()?.ResponseCode != ResponseCode.ServiceReady)
                        {
                            return new StartTlsResult(false, response1.Responses.Select(_ => _.ToString()).ToList(),
                                "The server did not present a service ready response code (220).");
                        }

                        EhloCommand ehloCommand = new EhloCommand(_mxSecurityTesterConfig.SmtpHostName);
                        _log.Debug($">: {ehloCommand.CommandString}");
                        await _smtpSerializer.Serialize(ehloCommand, streamWriter);
                        SmtpResponse response2 = await _smtpDeserializer.Deserialize(streamReader);
                        _log.Debug($"<: {response2}");
                        if (!response2.Responses.Any(_ =>
                            _.Value.ToLower() == Starttls && _.ResponseCode == ResponseCode.Ok))
                        {
                            return new StartTlsResult(false, response2.Responses.Select(_ => _.ToString()).ToList(),
                                "The server did not present a STARTTLS command with a response code (250).");
                        }

                        StartTlsCommand startTlsCommand = new StartTlsCommand();
                        _log.Debug($">: {startTlsCommand.CommandString}");
                        await _smtpSerializer.Serialize(startTlsCommand, streamWriter);
                        SmtpResponse response3 = await _smtpDeserializer.Deserialize(streamReader);
                        _log.Debug($"<: {response3}");

                        return new StartTlsResult(
                            response3.Responses.FirstOrDefault()?.ResponseCode == ResponseCode.ServiceReady,
                            response3.Responses.Select(_ => _.Value).ToList(), string.Empty);

                    }
                }
            }
            catch (Exception e)
            {
                _log.Error(
                    $"SMTP session initalization failed with error: {e.Message} {Environment.NewLine} {e.StackTrace}");
                return new StartTlsResult(false, null, e.Message);
            }
        }
    }
}