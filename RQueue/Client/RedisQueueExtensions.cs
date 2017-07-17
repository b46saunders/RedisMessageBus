using System;
using StackExchange.Redis;

namespace RQueue.Client
{
    public static class RedisQueueExtensions
    {
        public static  Queue GetQueue(this ConnectionMultiplexer connection,string queueName)
        {
            if (string.IsNullOrEmpty(queueName))
            {
                throw new ArgumentNullException($"{nameof(queueName)} cannot be null or empty");
            }
            return new Queue(connection,queueName);
        }
    }
}