using System;
using Newtonsoft.Json.Linq;
using RQueue.Shared;

namespace RQueue.Client
{
    public abstract class Job : IJob
    {
        public JRaw Payload { get; }

        protected Job(JRaw payload)
        {
            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
        }
    }
}