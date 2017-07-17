using Newtonsoft.Json.Linq;
using RQueue.Client.Interfaces;

namespace RQueue.Client
{
    public class AwaitableJobWithReturn<T,TReturn> : Job , IAwaitableJobWithReturn<T,TReturn>
    {
        internal AwaitableJobWithReturn(JRaw payload) : base(payload)
        {

        }
        
    }
}