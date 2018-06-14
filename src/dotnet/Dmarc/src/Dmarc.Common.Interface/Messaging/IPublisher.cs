using System.Threading.Tasks;

namespace Dmarc.Common.Interface.Messaging
{
    public interface IPublisher
    {
        Task Publish<T>(T message, string topic);
    }
}
