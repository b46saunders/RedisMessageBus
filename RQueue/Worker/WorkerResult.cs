namespace RQueue.Worker
{
    public class WorkerResult : IWorkerResult
    {
        public byte Status { get; }

        public WorkerResult(byte status)
        {
            Status = status;
        }
    }
}