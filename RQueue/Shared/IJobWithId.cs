namespace RQueue.Shared
{
    public interface IJobWithId : IJob
    {
        long JobId { get; }
    }
}