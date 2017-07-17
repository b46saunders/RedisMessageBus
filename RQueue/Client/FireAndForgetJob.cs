using Newtonsoft.Json.Linq;
using RQueue.Client.Interfaces;

namespace RQueue.Client
{
    public class FireAndForgetJob<T> : Job , IFireAndForgetJob<T>
    {
        internal FireAndForgetJob(JRaw payload) : base(payload)
        {
        }


    }
}