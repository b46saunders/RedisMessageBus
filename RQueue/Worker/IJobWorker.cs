using System.Threading.Tasks;

namespace RQueue.Worker
{
    public interface IJobWorker
    {
        Task<IRegisteredJobWorker> Start();
    }
}