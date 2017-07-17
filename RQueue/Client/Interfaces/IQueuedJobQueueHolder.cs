using System.Threading.Tasks;

namespace RQueue.Client.Interfaces
{
    public interface IQueuedJobQueueHolder<T>
    {
        IAwaitableQueuedJobHolder<T> AsAwaitable();

        Task<FireAndForgetQueuedJob<T>> AsFireAndForget();
    }
}