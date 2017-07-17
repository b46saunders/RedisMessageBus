using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RQueue.Client;
using RQueue.Shared;
using StackExchange.Redis;

namespace RQueue.Worker
{
    internal class AsyncJobProcessor<T> : IJobWorker, IRegisteredJobWorker
    {
        private readonly IConnectionMultiplexer connectionMultiplexer;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly string queueName;
        private readonly Func<IJobWithId, Task> handler;

        public static string DequeueScript =
            File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "dequeueJob.lua"));

        public AsyncJobProcessor(
            IConnectionMultiplexer connectionMultiplexer,
            string queueName,
            Func<IJobWithId, Task> handler
        )
        {
            this.connectionMultiplexer = connectionMultiplexer;
            cancellationTokenSource = new CancellationTokenSource();
            this.queueName = queueName;
            this.handler = handler;
        }


        private async Task StartAsync()
        {
            await ConsumeQueuedJobs();
            await SubscribeToAwaitingJobs();
        }

        private async Task SubscribeToAwaitingJobs()
        {
            await connectionMultiplexer.GetSubscriber()
                .SubscribeAsync(JobQueueConfig.GetNewlyAddedJobSubscriptionChannel(queueName), ((
                    channel, value) =>
                {
                    if (value.HasValue)
                    {
                        Task.Run(async () =>
                        {
                            try
                            {
                                await ConsumeQueuedJobs();
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        }, cancellationTokenSource.Token);
                    }
                }));
        }

        /// <summary>
        /// loops until all currently queued jobs are consumed
        /// </summary>
        /// <returns></returns>
        private async Task ConsumeQueuedJobs()
        {
            bool workFound;
            do
            {
                workFound = await TryPerformWork();
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
            } while (workFound);
        }

        private async Task<bool> TryPerformWork()
        {
            var workFromQueue = await GetWorkFromQueue();

            foreach (var queuedJobData in workFromQueue)
            {
                await handler(queuedJobData);
            }
            return workFromQueue.Any();
        }

        private async Task<IEnumerable<QueuedJobData>> GetWorkFromQueue()
        {
            //when we dequeue we need to pass in the key we are moving into the inprogress list
            var inProgressQueueKey = JobQueueConfig.GetInProgressQueueKey(queueName);

            var keys = new RedisKey[] { queueName, inProgressQueueKey };
            var result = await connectionMultiplexer.GetDatabase().ScriptEvaluateAsync(DequeueScript,keys);

            if (result.IsNull) return Enumerable.Empty<QueuedJobData>();

            var queuedJobData = JsonConvert.DeserializeObject<QueuedJobData>(result.ToString());
            return new[] { queuedJobData };
        }

        public void Deregister()
        {
            cancellationTokenSource.Cancel();
        }

        public async Task<IRegisteredJobWorker> Start()
        {
            await StartAsync();
            return this;
        }
    }
}