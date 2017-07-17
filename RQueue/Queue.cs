using System;
using System.Threading.Tasks;
using RQueue.Client;
using RQueue.Client.Interfaces;
using RQueue.Shared;
using RQueue.Worker;
using StackExchange.Redis;

namespace RQueue
{
    public class Queue
    {
        private ConnectionMultiplexer Connection { get; }
        public string QueueName { get; }

        public Queue(ConnectionMultiplexer connection,string queueName)
        {
            if (string.IsNullOrEmpty(queueName)) throw new ArgumentNullException(nameof(queueName));
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            QueueName = queueName;
        }

        public Task<long> GetJobId()
        {
            return Connection.GetDatabase().StringIncrementAsync(JobQueueConfig.GetQueueJobIdKey(QueueName));
        }

        public Task<AwaitableQueuedJobWithReturn<T,TReturn>> QueueAsync<T,TReturn>(IAwaitableJobWithReturn<T, TReturn> job)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            return QueuedJobBuilder<T>
                .Initialise(job as Job)
                .UsingConnection(Connection)
                .UsingIdProvider(GetJobId)
                .OnQueue(QueueName)
                .AsAwaitable()
                .WithReturn<TReturn>();
        }

        public Task<AwaitableQueuedJobWithoutReturn<T>> QueueAsync<T>(IAwaitableJobWithoutReturn<T> job)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            return  QueuedJobBuilder<T>
                .Initialise(job as Job)
                .UsingConnection(Connection)
                .UsingIdProvider(GetJobId)
                .OnQueue(QueueName)
                .AsAwaitable()
                .WithoutReturn();
        }

        public Task QueueAsync<T>(IFireAndForgetJob<T> job)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            return QueuedJobBuilder<T>
                .Initialise(job as Job)
                .UsingConnection(Connection)
                .UsingIdProvider(GetJobId)
                .OnQueue(QueueName)
                .AsFireAndForget();
        }

        public async Task<IRegisteredJobWorker> RegisterHandler<T, TResult>(Func<IJobWithId, IWorkerResultWithPayload<TResult>> handler)
        {
            var processor = JobWorkerBuilder.BuildAwaitableWithReturn<T,TResult>(Connection, QueueName, handler);
            return await processor.Start();
        }

        public async Task<IRegisteredJobWorker> RegisterHandler<T>(Func<IJobWithId, IWorkerResult> handler)
        {
            var processor = JobWorkerBuilder.BuildAwaitable<T>(Connection, QueueName, handler);
            return await processor.Start();
        }

        public async Task<IRegisteredJobWorker> RegisterHandler<T>(Action<IJobWithId> handler)
        {
            var processor = JobWorkerBuilder.BuildFireAndForget<T>(Connection, QueueName, handler);
            return await processor.Start();
        }
    }
}