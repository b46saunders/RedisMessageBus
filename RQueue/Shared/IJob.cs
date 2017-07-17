using Newtonsoft.Json.Linq;

namespace RQueue.Shared
{
    public interface IJob
    {
        JRaw Payload { get; }
    }
}