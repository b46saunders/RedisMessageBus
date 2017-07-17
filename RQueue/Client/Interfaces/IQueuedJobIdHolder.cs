namespace RQueue.Client.Interfaces
{
    public interface IQueuedJobIdHolder<T>
    {
        IQueuedJobQueueHolder<T> OnQueue(string queueName);
    }
}