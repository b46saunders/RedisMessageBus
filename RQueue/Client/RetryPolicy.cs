namespace RQueue.Client
{
    public class RetryPolicy
    {
        public int Times { get; }

        public RetryPolicy(int times)
        {
            Times = times;
        }
    }
}