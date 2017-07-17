using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RQueue.Client;
using RQueue.Shared;
using RQueue.Worker;
using StackExchange.Redis;

namespace RQueue.Test
{
    [TestClass]
    public class ReturnType
    {
        public string Foo { get; set; }
    }

    [TestClass]
    public class RQueueTest
    {

        
        [TestMethod]
        public async Task ClientApiTest()
        {
            var work = new DummyObject() { someData2 = "test", someIdThatWeNeed = 12, someData = "32" };

            var awaitableJobWithResult = JobBuilder<DummyObject>
                .Initialize()
                .WithPayload(new JRaw(JsonConvert.SerializeObject(work)))
                .WithRetry(new RetryPolicy(1))
                .AsAwaitable()
                .WithReturnResult<ReturnType>();

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

            var connection = await ConnectionMultiplexer.ConnectAsync("192.168.99.100:32768").ConfigureAwait(false);
            var queue = connection.GetQueue("tasks");

            var awaitableJobWithResultQueued = await queue.QueueAsync(awaitableJobWithResult);

            var awaitableJobWithoutResultQueued = await queue.QueueAsync(awaitableJobWithoutResult);
            await queue.QueueAsync(fireAndForgetJob);
            await awaitableJobWithoutResultQueued.AwaitCompletionAsync();

            var result = await awaitableJobWithResultQueued.AwaitResultAsync();
        }

        [TestMethod]
        public async Task WorkerApiTest()
        {
            var connection = await ConnectionMultiplexer.ConnectAsync("192.168.99.100:32768").ConfigureAwait(false);

            var status = connection.GetStatus();
            var queue = connection.GetQueue("tasks");

            var handler1 = await queue.RegisterHandler<DummyObject,int>(job =>
            {
                var dummyObject = JsonConvert.DeserializeObject<DummyObject>(job.Payload.ToString());
                Debug.WriteLine($"Received dummy object : {dummyObject}");
                return new ScalarWorkerResult<int>(dummyObject.someIdThatWeNeed, 12);
            });

            var handler2 = await queue.RegisterHandler<DummyObject>(job =>
            {
                var payload = job.Payload.ToString();
                return new WorkerResult(10);
            });

            var handler3 = await queue.RegisterHandler<DummyObject>(job =>
            {
                var payload = job.Payload.ToString();
                Console.WriteLine(job.Payload);
            });

            handler1.Deregister();
            handler2.Deregister();
            handler3.Deregister();


        }
    }
}
