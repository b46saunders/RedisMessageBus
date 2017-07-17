using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RQueue.Client;
using RQueue.Shared;
using RQueue.Worker;
using StackExchange.Redis;

namespace ConsoleClient
{
    class Program
    {
        static void Main()
        {
            Run();
            Console.ReadLine();
        }


        public static async Task Run()
        {

            var work = new DummyObject() { someData2 = "test", someIdThatWeNeed = 12, someData = "32" };
            var withResultQueueName = "tasksWithResult";

            var awaitableJobWithResult = JobBuilder<DummyObject>
                .Initialize()
                .WithPayload(new JRaw(JsonConvert.SerializeObject(work)))
                .WithRetry(new RetryPolicy(1))
                .AsAwaitable()
                .WithReturnResult<ScalarWorkerResult<int>>();

            var awaitableJobWithoutResult = JobBuilder<DummyObject>
                .Initialize()
                .WithPayload(new JRaw(JsonConvert.SerializeObject(work)))
                .WithRetry(new RetryPolicy(1))
                .AsAwaitable()
                .WithoutReturnResult();

            var fireAndForgetJob = JobBuilder<DummyObject>
                .Initialize()
                .WithPayload(new JRaw(JsonConvert.SerializeObject(work)))
                .WithRetry(new RetryPolicy(1))
                .AsFireAndForget();

            //Connection to docker container with redis port exposed
            var connection = await ConnectionMultiplexer.ConnectAsync("192.168.99.100:32768").ConfigureAwait(false);

            var queueName = "tasks";

            await ClearOldData(connection, withResultQueueName);
            var queue = connection.GetQueue(withResultQueueName);

            //var awaitableJobWithResultQueued = await queue.QueueAsync(awaitableJobWithResult);

            //var awaitableJobWithoutResultQueued = await queue.QueueAsync(awaitableJobWithoutResult);

            do
            {
                Console.WriteLine("Queuing job");
                var queuedJob = await queue.QueueAsync(awaitableJobWithResult);
                var awaitedResult = await queuedJob.AwaitResultAsync();

                Console.WriteLine($"Received result from worker of : {awaitedResult.Result} with status {awaitedResult.Status}");
                //await GetCountsInQueues(connection, withResultQueueName);
                //await Task.Delay(0);
            } while (true);
            
            //await awaitableJobWithoutResultQueued.AwaitCompletionAsync();

            //var result = await awaitableJobWithResultQueued.AwaitResultAsync();
        }

        public static async Task GetCountsInQueues(ConnectionMultiplexer connection,string queueName)
        {
            var result = await connection.GetDatabase().ListLengthAsync(queueName);
            Console.WriteLine($"Jobs in queue : {result}");
        }

        public static async Task ClearOldData(ConnectionMultiplexer connection,string queueName)
        {
            Console.WriteLine("Clearing existing queues before testing : ");
            Console.WriteLine(JobQueueConfig.GetInProgressQueueKey(queueName));
            Console.WriteLine(JobQueueConfig.GetQueueJobIdKey(queueName));
            Console.WriteLine(queueName);

            var db = connection.GetDatabase();
            var r1 = await db.KeyDeleteAsync(JobQueueConfig.GetInProgressQueueKey(queueName));
            var r2 = await db.KeyDeleteAsync(JobQueueConfig.GetQueueJobIdKey(queueName));
            var r3 = await db.KeyDeleteAsync(queueName);
        }
    }
}