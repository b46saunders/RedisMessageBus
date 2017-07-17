using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RQueue.Client.Interfaces;
using RQueue.Shared;
using RQueue.Worker;
using StackExchange.Redis;

namespace RQueue.Client
{
    public class QueuedJobBuilder<T> : IQueuedJobWithoutConnection<T>,IQueuedJobConnectionHolder<T> , IQueuedJobIdHolder<T>, IQueuedJobQueueHolder<T>, IAwaitableQueuedJobHolder<T>
    {
        private string QueueName { get; set; }
        private IConnectionMultiplexer Connection { get; set; }
        private Func<Task<long>> IdProvider { get; set; } 
        private Job Job { get; set; }


        public static IQueuedJobWithoutConnection<T> Initialise(Job job)
        {
            return new QueuedJobBuilder<T>()
            {
                Job = job
            };
        }

        public IQueuedJobConnectionHolder<T> UsingConnection(IConnectionMultiplexer connection)
        {
            return new QueuedJobBuilder<T>()
            {
                Connection = connection,
                Job = Job
            };
        }

        public IQueuedJobIdHolder<T> UsingIdProvider(Func<Task<long>> idProvider)
        {
            return new QueuedJobBuilder<T>()
            {
                Connection = Connection,
                IdProvider = idProvider,
                Job = Job
            };
        }

        public IQueuedJobQueueHolder<T> OnQueue(string queueName)
        {
            return new QueuedJobBuilder<T>()
            {
                Connection = Connection,
                QueueName = queueName,
                Job = Job,
                IdProvider = IdProvider
            };
        }

        public IAwaitableQueuedJobHolder<T> AsAwaitable()
        {
            return new QueuedJobBuilder<T>()
            {
                Connection = Connection,
                QueueName = QueueName,
                Job = Job,
                IdProvider = IdProvider
            };
        }

        public async Task<AwaitableQueuedJobWithoutReturn<T>> WithoutReturn()
        {
            var jobId = await IdProvider();
            var taskCompletionSource = new TaskCompletionSource<byte>();
            await Connection.GetSubscriber().SubscribeAsync(JobQueueConfig.GetReturnQueueSubscriptionChannel(QueueName, jobId),
                (channel, value) =>
                {
                    var workerResult = JsonConvert.DeserializeObject<WorkerResult>(value);
                    taskCompletionSource.SetResult(workerResult.Status);
                });

            var queuedJob = new AwaitableQueuedJobWithoutReturn<T>(Job, jobId, taskCompletionSource.Task);
            await QueueJob(jobId);
            
            return queuedJob;
        }

        public async Task<AwaitableQueuedJobWithReturn<T,TReturn>> WithReturn<TReturn>()
        {
            var jobId = await IdProvider();

            var taskCompletionSource = new TaskCompletionSource<TReturn>();
            
            await Connection.GetSubscriber().SubscribeAsync(JobQueueConfig.GetReturnQueueSubscriptionChannel(QueueName,jobId),
                (channel, value) =>
                {
                    taskCompletionSource.SetResult(JsonConvert.DeserializeObject<TReturn>(value));
                });
            
            var queuedJob = new AwaitableQueuedJobWithReturn<T, TReturn>(Job, jobId, taskCompletionSource.Task);
            
            await QueueJob(jobId);
            return queuedJob;
        }
        
        public async Task<FireAndForgetQueuedJob<T>> AsFireAndForget()
        {
            var jobId = await IdProvider();
            var queuedJob = new FireAndForgetQueuedJob<T>(Job, jobId);
            await QueueJob(jobId);
            return queuedJob;
        }

        private async Task QueueJob(long jobId)
        {
            var jobData = new QueuedJobData(Job, jobId);
            await Connection.GetDatabase().ListRightPushAsync(QueueName, JsonConvert.SerializeObject(jobData));
            await Connection.GetSubscriber()
                .PublishAsync(JobQueueConfig.GetNewlyAddedJobSubscriptionChannel(QueueName), jobId);
        }
    }
}