using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Dmarc.Common.Encryption
{
    public class AmazonEncryptionUtils
    {
        public static async Task<string> DecodeEnvironmentVariableAsync(string environmentVariableName)
        {
            var encryptedBase64Text = System.Environment.GetEnvironmentVariable(environmentVariableName);
            var encryptedBytes = Convert.FromBase64String(encryptedBase64Text);
            using (var client = new AmazonKeyManagementServiceClient())
            {
                var decryptRequest = new DecryptRequest
                {
                    CiphertextBlob = new MemoryStream(encryptedBytes),
                };
                
                var response = await client.DecryptAsync(decryptRequest).ConfigureAwait(false);
                using (var plaintextStream = response.Plaintext)
                {
                    return Encoding.UTF8.GetString(plaintextStream.ToArray());
                }
            }
        }
    }
}
