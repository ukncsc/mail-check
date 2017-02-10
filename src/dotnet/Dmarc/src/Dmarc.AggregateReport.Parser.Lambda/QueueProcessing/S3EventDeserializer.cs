using System;
using Amazon.Lambda.S3Events;
using Newtonsoft.Json;

namespace Dmarc.AggregateReport.Parser.Lambda.QueueProcessing
{
    internal interface IS3EventDeserializer
    {
        bool TryDeserializeS3Event(string serializedObject, out S3Event s3Event);
    }

    internal class S3EventDeserializer : IS3EventDeserializer
    {
        public bool TryDeserializeS3Event(string serializedObject, out S3Event s3Event)
        {
            try
            {
                s3Event = JsonConvert.DeserializeObject<S3Event>(serializedObject);
                return s3Event.Records != null;
            }
            catch (Exception)
            {
                s3Event = null;
                return false;
            }
        }
    }
}