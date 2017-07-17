using RQueue.Shared;

namespace RQueue.Client.Interfaces
{
    public interface IAwaitableJobWithReturn<T, TReturn> : IJob
    {
        
    }
}