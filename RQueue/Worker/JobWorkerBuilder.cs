using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RQueue.Client;
using RQueue.Shared;
using StackExchange.Redis;

namespace RQueue.Worker
{
    public class JobWorkerBuilder
    {
        
        public static IJobWorker BuildAwaitableWithReturn<T,TResult>(
            IConnectionMultiplexer connectionMultiplexer,
            string queueName, Func<IJobWithId,
                IWorkerResultWithPayload<TResult>> handler)
        {
            var c = new Func<IJobWithId, Task>(async job =>
            {

                var result = handler(job);
                await connectionMultiplexer.GetSubscriber()
                    .PublishAsync(JobQueueConfig.GetReturnQueueSubscriptionChannel(queueName, job.JobId),
                        JsonConvert.SerializeObject(result));
            });
            return new AsyncJobProcessor<T>(connectionMultiplexer,queueName,c);
        }

        public static IJobWorker BuildAwaitable<T>(
            IConnectionMultiplexer connectionMultiplexer,
            string queueName, Func<IJobWithId, IWorkerResult> handler)
        {
            var c = new Func<IJobWithId, Task>(async job =>
            {
                var result = handler(job);
                await connectionMultiplexer.GetSubscriber()
                    .PublishAsync(JobQueueConfig.GetReturnQueueSubscriptionChannel(queueName, job.JobId),
                        JsonConvert.SerializeObject(result));
            });
            return new AsyncJobProcessor<T>(connectionMultiplexer, queueName, c);
        }

        public static IJobWorker BuildFireAndForget<T>(
            IConnectionMultiplexer connectionMultiplexer,
            string queueName, Action<IJobWithId> handler)
        {
            var c = new Func<IJobWithId, Task>(job =>
            {
                handler(job);
                return Task.CompletedTask;
            });
            return new AsyncJobProcessor<T>(connectionMultiplexer, queueName, c);
        }
    }
}