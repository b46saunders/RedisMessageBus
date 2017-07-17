using Newtonsoft.Json.Linq;

namespace RQueue.Client.Interfaces
{
    public interface IJobWithoutPayload<T>
    {
        IJobWithPayload<T> WithPayload(JRaw payload);
    }
}