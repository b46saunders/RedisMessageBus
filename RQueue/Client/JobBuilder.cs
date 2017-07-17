using Newtonsoft.Json.Linq;
using RQueue.Client.Interfaces;

namespace RQueue.Client
{
    public class JobBuilder<T> : IJobWithoutPayload<T>, IJobWithPayloadAndRetryHolder<T> , IJobWithPayload<T> , IAwaitableJobHolder<T>
    {
        private JRaw PayLoad { get; set; }
        private RetryPolicy RetryPolicy { get; set; }

        public static IJobWithoutPayload<T> Initialize()
        {
            return new JobBuilder<T>();
        }

        public IJobWithPayload<T> WithPayload(JRaw payload)
        {
            return new JobBuilder<T>()
            {
                PayLoad = payload
            };
        }

        public IAwaitableJobHolder<T> AsAwaitable()
        {
            return new JobBuilder<T>
            {
                PayLoad = PayLoad,
                RetryPolicy = RetryPolicy
            };
        }

        public FireAndForgetJob<T> AsFireAndForget()
        {
            return new FireAndForgetJob<T>(PayLoad);
        }

        public IJobWithPayloadAndRetryHolder<T> WithRetry(RetryPolicy retryPolicy)
        {
            return new JobBuilder<T>()
            {
                RetryPolicy = retryPolicy,
                PayLoad = PayLoad
            };
        }

        public IAwaitableJobWithoutReturn<T> WithoutReturnResult()
        {
            return new AwaitableJobWithoutReturn<T>(PayLoad);
        }

        public IAwaitableJobWithReturn<T, TReturn> WithReturnResult<TReturn>()
        {
            return new AwaitableJobWithReturn<T, TReturn>(PayLoad);
        }
    }
}