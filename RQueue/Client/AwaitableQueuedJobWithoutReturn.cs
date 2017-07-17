using System;
using System.Threading.Tasks;
using RQueue.Client.Interfaces;
using RQueue.Shared;

namespace RQueue.Client
{
    public class AwaitableQueuedJobWithoutReturn<T> : QueuedJob<T> , IAwaitableQueuedJobWithoutReturn<T>
    {
        private readonly Task<byte> completedTask;

        public AwaitableQueuedJobWithoutReturn(IJob job,long jobId,Task<byte> completedTask) : base(job, jobId)
        {
            this.completedTask = completedTask ?? throw new ArgumentNullException(nameof(completedTask));
        }

        public Task AwaitCompletionAsync()
        {
            return completedTask;
        }
    }
}