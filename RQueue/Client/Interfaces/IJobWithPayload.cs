namespace RQueue.Client.Interfaces
{
    public interface IJobWithPayload<T>
    {
        IJobWithPayloadAndRetryHolder<T> WithRetry(RetryPolicy retryPolicy);
    }
}