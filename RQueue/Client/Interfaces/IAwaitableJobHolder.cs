namespace RQueue.Client.Interfaces
{
    public interface IAwaitableJobHolder<T>
    {
        IAwaitableJobWithoutReturn<T> WithoutReturnResult();
        IAwaitableJobWithReturn<T, TReturn> WithReturnResult<TReturn>(); 
    }
}