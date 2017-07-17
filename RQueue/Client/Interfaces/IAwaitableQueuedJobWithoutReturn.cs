using System.Threading.Tasks;

namespace RQueue.Client.Interfaces
{
    public interface IAwaitableQueuedJobWithoutReturn<T>
    {
        Task AwaitCompletionAsync();
    }
}