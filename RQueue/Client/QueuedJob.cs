using System;
using RQueue.Shared;

namespace RQueue.Client
{
    public abstract class QueuedJob<T>
    {
        public IJob Job { get; }
        public long JobId { get; }

        protected QueuedJob(IJob job, long jobId)
        {
            JobId = jobId;
            Job = job ?? throw new ArgumentNullException(nameof(job));
        }

    }
}