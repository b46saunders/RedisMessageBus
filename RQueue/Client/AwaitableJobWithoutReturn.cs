using Newtonsoft.Json.Linq;
using RQueue.Client.Interfaces;

namespace RQueue.Client
{
    public class AwaitableJobWithoutReturn<T> : Job, IAwaitableJobWithoutReturn<T>
    {
        internal AwaitableJobWithoutReturn(JRaw payload) : base(payload)
        {
        }
    }
}