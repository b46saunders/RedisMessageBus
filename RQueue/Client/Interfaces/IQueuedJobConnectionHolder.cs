using System;
using System.Threading.Tasks;

namespace RQueue.Client.Interfaces
{
    public interface IQueuedJobConnectionHolder<T>
    {
        IQueuedJobIdHolder<T> UsingIdProvider(Func<Task<long>> idProvider);
    }
}