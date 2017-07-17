using System;
using System.Threading.Tasks;
using RQueue.Client.Interfaces;
using RQueue.Shared;

namespace RQueue.Client
{
    public class AwaitableQueuedJobWithReturn<T, TReturn> : QueuedJob<T> , IAwaitableQueuedJobWithReturn<T,TReturn>
    {
        private readonly Task<TReturn> completedTask;

        public AwaitableQueuedJobWithReturn(IJob job, long jobId,Task<TReturn> completedTask) : base(job, jobId)
        {
            this.completedTask = completedTask ?? throw new ArgumentNullException(nameof(completedTask));
        }

        public Task<TReturn> AwaitResultAsync()
        {
            return completedTask;
        }
    }
}