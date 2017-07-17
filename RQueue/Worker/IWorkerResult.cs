
namespace RQueue.Worker
{

    public interface IWorkerResultWithPayload<out T> : IWorkerResult
    {
        T Result { get; }
    }

    public interface IWorkerResult
    {
        byte Status { get; }
    }
}