using System.Threading.Tasks;

namespace RQueue.Client.Interfaces
{
    public interface IAwaitableQueuedJobHolder<T>
    {
        Task<AwaitableQueuedJobWithoutReturn<T>> WithoutReturn();
        Task<AwaitableQueuedJobWithReturn<T,TReturn>> WithReturn<TReturn>();
    }
}