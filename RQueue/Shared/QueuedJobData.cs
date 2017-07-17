using System;
using Newtonsoft.Json.Linq;
using RQueue.Client;

namespace RQueue.Shared
{
    internal class QueuedJobData : IJobWithId {
        public long JobId { get; set; }
        public JRaw Payload { get; set; }

        public QueuedJobData()
        {
            
        }

        public QueuedJobData(Job job,long jobId)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            JobId = jobId;
            Payload = job.Payload;
        }
    }
}