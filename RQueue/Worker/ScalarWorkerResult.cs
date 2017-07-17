namespace RQueue.Worker
{
    public class ScalarWorkerResult<T> : IWorkerResultWithPayload<T> where T : struct 
    {
        public byte Status { get; }
        public T Result { get; }

        public ScalarWorkerResult(T result,byte status)
        {
            Result = result;
            Status = status;
        }
    }
}