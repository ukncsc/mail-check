using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;
using System.Threading.Tasks;

namespace Dmarc.Common.Encryption
{
    public interface IParameterStoreRequest
    {
        Task<string> GetParameterValue(string key);
    }

    public class ParameterStoreRequest : IParameterStoreRequest
    {
        private readonly IAmazonSimpleSystemsManagement _simpleSystemsManagement;

        public ParameterStoreRequest(IAmazonSimpleSystemsManagement simpleSystems)
        {
            _simpleSystemsManagement = simpleSystems;
        }

        public async Task<string> GetParameterValue(string key)
        {
            var parametersRequest = new GetParameterRequest { Name = key, WithDecryption = true };
            var responseTask = await _simpleSystemsManagement.GetParameterAsync(parametersRequest);

            return responseTask.Parameter.Value;
        }
    }
}
