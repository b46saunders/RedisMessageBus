using StackExchange.Redis;

namespace RQueue.Client.Interfaces
{
    public interface IQueuedJobWithoutConnection<T>
    {
        IQueuedJobConnectionHolder<T> UsingConnection(IConnectionMultiplexer connection);
    }
}