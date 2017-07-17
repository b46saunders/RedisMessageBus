namespace RedisMessageBus.Server
{
    public class Job
    {
        public long JobId { get; set; }
        public string Payload { get; set; }
    }
}