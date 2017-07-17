namespace RQueue.Client.Interfaces
{
    public interface IJobWithPayloadAndRetryHolder<T>
    {
        IAwaitableJobHolder<T> AsAwaitable();
        FireAndForgetJob<T> AsFireAndForget();
    }
}