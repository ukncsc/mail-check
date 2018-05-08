using System;
using System.Threading.Tasks;

namespace Dmarc.Common.Interface.Messaging
{
    public interface IQueueProcessor<T>
    {
        Task Run(Func<T, Task> job);
    }
}

