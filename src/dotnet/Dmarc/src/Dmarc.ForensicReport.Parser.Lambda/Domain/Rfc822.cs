using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace Dmarc.ForensicReport.Parser.Lambda.Domain
{
    public class Rfc822 : EmailPart
    {
        public Rfc822(string contentType, int depth, Disposition disposition) 
            : base(contentType, depth, disposition)
        {
        }

        //(rfc5322)
        //3.6.1 Occurences 1-1
        public DateTime? Date { get; set; }
        //3.6.6 Occurences 0-*
        public List<DateTime> ResentDate { get; set; } = new List<DateTime>();
        //3.6.7 Occurences 0-*
        public List<ReceivedHeader> Received { get; set; } = new List<ReceivedHeader>();
        //3.6.7 Occurences 0-1 relaxed as have seen multiple in data
        public List<MailAddress> ReturnPath { get; set; } = new List<MailAddress>();
        //3.6.2 Occurences 1-1 of field but field can be comma separated list of 1 or more addresses
        public MailAddressCollection From { get; set; }
        //3.6.6 Occurences 0-*
        public MailAddressCollection ResentFrom { get; set; }
        //3.6.2 Occurences 0-1 single value
        public MailAddress Sender { get; set; }
        //3.6.6 Occurences 0-*
        public MailAddressCollection ResentSender { get; set; }
        //3.6.2 Occurences 0-1
        public MailAddressCollection ReplyTo { get; set; }
        //3.6.3 Occurences 0-1 of field but comma separated list of 1 or more addresses
        public MailAddressCollection To { get; set; }
        //3.6.6 Occurences 0-*
        public MailAddressCollection ResentTo { get; set; }
        //3.6.3 Occurences 0-1 of field but comma separated list of 1 or more addresses
        public MailAddressCollection Cc { get; set; }
        //3.6.6 Occurences 0-*
        public MailAddressCollection ResentCc { get; set; }
        //3.6.3 Occurences 0-1 of field but comma separated list of 1 or more addresses
        public MailAddressCollection Bcc { get; set; }
        //3.6.6 Occurences 0-*
        public MailAddressCollection ResentBcc { get; set; }
        //3.6.4 Occurences 0-1 single unique message id
        public MailAddress MessageId { get; set; }
        //3.6.6 Occurences 0-*
        public List<MailAddress> ResentMessageId { get; set; } = new List<MailAddress>();
        ///3.6.4 Occurences 0-1 multiple unique message ids
        public MailAddressCollection InReplyTo { get; set; }
        ///3.6.4 Occurences 0-1 multiple unique message ids
        public MailAddressCollection References { get; set; }
        //3.6.5 Occurences 0-1 
        public string Subject { get; set; }
        //3.6.5 Occurences 0-*
        public List<string> Comments { get; set; } = new List<string>();
        //3.6.5 Occurences 0-*
        public List<string> Keywords { get; set; } = new List<string>();

        //Other
        public string AuthenticationResults { get; set; }
        public string ReceivedSpf { get; set; }
        public string UserAgent { get; set; }
        public string DkimSignature { get; set; }

        //Should be a single field but examples with multiple are common
        public List<IPAddress> XOrigninatingIp { get; set; } = new List<IPAddress>();
    }
}