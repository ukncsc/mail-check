using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dmarc.MxSecurityTester.Util
{
    public class X509CertificateConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            List<X509Certificate2> certificates = value as List<X509Certificate2>;

            if (certificates != null)
            {
                List<string> rawDataCertificates = certificates.Select(_ => Convert.ToBase64String(_.RawData)).ToList();
                JToken.FromObject(rawDataCertificates).WriteTo(writer);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<X509Certificate2> certificates = new List<X509Certificate2>();

            string[] rawRertificates = JsonConvert.DeserializeObject<string[]>(JToken.Load(reader).ToString());

            if (rawRertificates != null)
            {
                certificates = rawRertificates.Select(_ => new X509Certificate2(Convert.FromBase64String(_))).ToList();
            }

            return certificates;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(X509Certificate2);
        }
    }
}
