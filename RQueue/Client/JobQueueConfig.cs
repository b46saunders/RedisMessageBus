using StackExchange.Redis;

namespace RQueue.Client
{
    public static class JobQueueConfig
    {
        public static string GetReturnQueueSubscriptionChannel(string queueName,long jobId)
        {
            return $"awaitJobSubscription:{queueName}:{jobId}";
        }

        public static string GetInProgressQueueKey(string queueName)
        {
            return $"inProgressQueue:{queueName}";
        }

        public static string GetNewlyAddedJobSubscriptionChannel(string queueName)
        {
            return $"newlyAddedJobSubscription:{queueName}";
        }

        public static RedisKey GetQueueJobIdKey(string queueName)
        {
            return $"queueJobIdIncrby:{queueName}";
        }
    }
}