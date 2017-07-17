using RQueue.Client.Interfaces;
using RQueue.Shared;

namespace RQueue.Client
{
    public class FireAndForgetQueuedJob<T> : QueuedJob<T>, IFireAndForgetQueuedJob<T>
    {
        public FireAndForgetQueuedJob(IJob job, long jobId) : base(job, jobId)
        {
        }
    }
}