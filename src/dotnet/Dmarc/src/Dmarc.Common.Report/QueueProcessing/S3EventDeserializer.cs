using System;
using System.Linq;
using Amazon.Lambda.S3Events;
using Newtonsoft.Json;

namespace Dmarc.Common.Report.QueueProcessing
{
    public interface IS3EventDeserializer
    {
        bool TryDeserializeS3Event(string serializedObject, out S3Event s3Event);
    }

    public class S3EventDeserializer : IS3EventDeserializer
    {
        public bool TryDeserializeS3Event(string serializedObject, out S3Event s3Event)
        {
            try
            {
                s3Event = JsonConvert.DeserializeObject<S3Event>(serializedObject);
                return s3Event.Records?.Any() ?? false;
            }
            catch (Exception)
            {
                s3Event = null;
                return false;
            }
        }
    }
}