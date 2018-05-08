using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Dmarc.ForensicReport.Parser.Lambda.Dao.Entities;
using Dmarc.ForensicReport.Parser.Lambda.Domain;

namespace Dmarc.ForensicReport.Parser.Lambda.Converters
{
    public interface IRfc822ToEntityConverter
    {
        Rfc822HeaderSetEntity Convert(Rfc822 rfc822, int order);
    }

    public class Rfc822ToEntityConverter : IRfc822ToEntityConverter
    {
        private readonly IIpAddressToEntityConverter _ipAddressToEntityConverter;

        public Rfc822ToEntityConverter(IIpAddressToEntityConverter ipAddressToEntityConverter)
        {
            _ipAddressToEntityConverter = ipAddressToEntityConverter;
        }

        public Rfc822HeaderSetEntity Convert(Rfc822 rfc822, int order)
        {
            ContentTypeEntity contentTypeEntity = new ContentTypeEntity(rfc822.ContentType);
            List<Rfc822HeaderEntity> headers = Convert(rfc822);
            return new Rfc822HeaderSetEntity(contentTypeEntity, order, rfc822.Depth, headers);
        }

        private List<Rfc822HeaderEntity> Convert(Rfc822 rfc822)
        {
            List<Rfc822HeaderEntity> rfc822HeaderEntities = new List<Rfc822HeaderEntity>();
        
            if (rfc822.Date.HasValue)
            {
                    rfc822HeaderEntities.Add(CreateDateHeader("date", 0, rfc822.Date.Value));
            }
        
            if (rfc822.ResentDate.Any())
            {
                rfc822HeaderEntities.AddRange(rfc822.ResentDate.Select((value,index) => CreateDateHeader("resent-date", index, value)));
            }
        
            if (rfc822.Received.Any())
            {
                rfc822HeaderEntities.AddRange(rfc822.Received.SelectMany(CreateReceivedFields));
            }
        
            if (rfc822.ReturnPath != null && rfc822.ReturnPath.Any())
            {
                rfc822HeaderEntities.AddRange(rfc822.ReturnPath.Select((value, index) => CreateEmailAddressHeader("return-path", index, value)));
            }
        
            if (rfc822.From != null && rfc822.From.Any())
            {
                rfc822HeaderEntities.AddRange(rfc822.From.Select((value, index) => CreateEmailAddressHeader("from", index, value)));
            }
        
            if (rfc822.ResentFrom != null && rfc822.ResentFrom.Any())
            {
                rfc822HeaderEntities.AddRange(rfc822.ResentFrom.Select((value, index) => CreateEmailAddressHeader("resent-from", index, value)));
            }
        
            if (rfc822.Sender != null)
            {
                rfc822HeaderEntities.Add(CreateEmailAddressHeader("sender", 0, rfc822.Sender));
            }
        
            if (rfc822.ResentSender != null && rfc822.ResentSender.Any())
            {
                rfc822HeaderEntities.AddRange(rfc822.ResentSender.Select((value, index) => CreateEmailAddressHeader("resent-sender", index, value)));
            }
        
            if (rfc822.ReplyTo != null && rfc822.ReplyTo.Any())
            {
                rfc822HeaderEntities.AddRange(rfc822.ReplyTo.Select((value, index) => CreateEmailAddressHeader("reply-to", index, value)));
            }
        
            if (rfc822.To != null && rfc822.To.Any())
            {
                rfc822HeaderEntities.AddRange(rfc822.To.Select((value, index) => CreateEmailAddressHeader("to", index, value)));
            }
        
            if(rfc822.ResentTo != null && rfc822.ResentTo.Any())
            {
                rfc822HeaderEntities.AddRange(rfc822.ResentTo.Select((value, index) => CreateEmailAddressHeader("resent-to", index, value)));
            }
        
            if (rfc822.Cc != null && rfc822.Cc.Any())
            {
                rfc822HeaderEntities.AddRange(rfc822.Cc.Select((value, index) => CreateEmailAddressHeader("cc", index, value)));
            }
        
            if (rfc822.ResentCc != null && rfc822.ResentCc.Any())
            {
                rfc822HeaderEntities.AddRange(rfc822.ResentCc.Select((value, index) => CreateEmailAddressHeader("resent-cc", index, value)));
            }
        
            if (rfc822.Bcc != null && rfc822.Bcc.Any())
            {
                rfc822HeaderEntities.AddRange(rfc822.Bcc.Select((value, index) => CreateEmailAddressHeader("bcc", index, value)));
            }
        
            if (rfc822.ResentCc != null && rfc822.ResentCc.Any())
            {
                rfc822HeaderEntities.AddRange(rfc822.ResentCc.Select((value, index) => CreateEmailAddressHeader("resent-bcc", index, value)));
            }
        
            if (rfc822.MessageId != null)
            {
                rfc822HeaderEntities.Add(CreateTextHeader("message-id", 0, rfc822.MessageId.Address));
            }
        
            if (rfc822.ResentMessageId.Any())
            {
                rfc822HeaderEntities.AddRange(rfc822.ResentMessageId.Select((value, index) => CreateTextHeader("resent-message-id", index, value.Address)));
            }
        
            if (rfc822.InReplyTo != null && rfc822.InReplyTo.Any())
            {
                rfc822HeaderEntities.AddRange(rfc822.InReplyTo.Select((value, index) => CreateTextHeader("in-reply-to", index, value.Address)));
            }
        
            if (rfc822.References != null && rfc822.References.Any())
            {
                rfc822HeaderEntities.AddRange(rfc822.References.Select((value, index) => CreateTextHeader("references", index, value.Address)));
            }
        
            if (!string.IsNullOrEmpty(rfc822.Subject))
            {
                rfc822HeaderEntities.Add(CreateTextHeader("subject", 0, rfc822.Subject));
            }
        
            if (rfc822.Comments.Any())
            {
                rfc822HeaderEntities.AddRange(rfc822.Comments.Select((value, index) => CreateTextHeader("comments", index, value)));
            }
        
            if (rfc822.Keywords.Any())
            {
                rfc822HeaderEntities.AddRange(rfc822.Keywords.Select((value, index) => CreateTextHeader("keywords", index, value)));
            }
        
            if (!string.IsNullOrEmpty(rfc822.AuthenticationResults))
            {
                rfc822HeaderEntities.Add(CreateTextHeader("authentication-results", 0, rfc822.AuthenticationResults));
            }
        
            if (!string.IsNullOrEmpty(rfc822.ReceivedSpf))
            {
                rfc822HeaderEntities.Add(CreateTextHeader("received-spf", 0, rfc822.ReceivedSpf));
            }
        
            if (!string.IsNullOrEmpty(rfc822.UserAgent))
            {
                rfc822HeaderEntities.Add(CreateTextHeader("user-agent", 0, rfc822.UserAgent));
            }
        
            if (!string.IsNullOrEmpty(rfc822.DkimSignature))
            {
                rfc822HeaderEntities.Add(CreateTextHeader("dkim-signature", 0, rfc822.DkimSignature));
            }
        
            if (rfc822.XOrigninatingIp.Any())
            {
                rfc822HeaderEntities.AddRange(rfc822.XOrigninatingIp.Select((value, index) => CreateIpHeader("x-origininating-ip", index, value)));
            }
        
            return rfc822HeaderEntities;
        }

        private IEnumerable<Rfc822HeaderEntity> CreateReceivedFields(ReceivedHeader receivedHeader, int index)
        {
            if (receivedHeader.From != null)
            {
                yield return CreateTextHeader("received_from", index, receivedHeader.From);
            }

            if (receivedHeader.By != null)
            {
                yield return CreateTextHeader("received_by", index, receivedHeader.By);
            }

            if (receivedHeader.For != null)
            {
                yield return CreateEmailAddressHeader("received_for", index, receivedHeader.For);
            }
        }

        private Rfc822HeaderEntity CreateDateHeader(string name, int order, DateTime dateTime)
        {
            Rfc822HeaderFieldEntity rfc822HeaderFieldEntity = new Rfc822HeaderFieldEntity(name, EntityRfc822HeaderValueType.Date);

            return new Rfc822HeaderEntity(rfc822HeaderFieldEntity, order, null, null, null, dateTime, null);
        }

        private Rfc822HeaderEntity CreateEmailAddressHeader(string name, int order, MailAddress emailAddress)
        {
            Rfc822HeaderFieldEntity rfc822HeaderFieldEntity = new Rfc822HeaderFieldEntity(name, EntityRfc822HeaderValueType.Email);

            EmailAddressEntity emailAddressEntity = new EmailAddressEntity(emailAddress.Address);

            return new Rfc822HeaderEntity(rfc822HeaderFieldEntity, order, null, emailAddressEntity, null, null, null);
        }

        private Rfc822HeaderEntity CreateTextHeader(string name, int order, string text)
        {
            Rfc822HeaderFieldEntity rfc822HeaderFieldEntity = new Rfc822HeaderFieldEntity(name, EntityRfc822HeaderValueType.Text);

            Rfc822HeaderTextValueEntity rfc822HeaderTextValueEntity = new Rfc822HeaderTextValueEntity(text);

            return new Rfc822HeaderEntity(rfc822HeaderFieldEntity, order, rfc822HeaderTextValueEntity, null, null, null, null);
        }

        private Rfc822HeaderEntity CreateIpHeader(string name, int order, IPAddress ipAddress)
        {
            Rfc822HeaderFieldEntity rfc822HeaderFieldEntity = new Rfc822HeaderFieldEntity(name, EntityRfc822HeaderValueType.Ip);

            IpAddressEntity ipAddressEntity = _ipAddressToEntityConverter.Convert(ipAddress);

            return new Rfc822HeaderEntity(rfc822HeaderFieldEntity, order, null, null, ipAddressEntity, null, null);
        }
    }
}