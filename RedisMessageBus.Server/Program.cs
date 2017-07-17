using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;
using Newtonsoft.Json;
using RQueue.Client;


namespace RedisMessageBus.Server
{
    class Program
    {
        public static ConnectionMultiplexer Redis = ConnectionMultiplexer.Connect("192.168.99.100:32768");
        public static readonly string JobIdIndexKey = "jobIdIndex";

        public static string DequeueScript = File.ReadAllText("dequeueJob.lua");
        static void Main(string[] args)
        {
            

            Run();
            Console.ReadLine();

            
        }

        public static void Cleanup(ConnectionMultiplexer redis)
        {
            var queueName = "tasks";
            var db = redis.GetDatabase();
            var r1 =db.KeyDelete(JobQueueConfig.GetInProgressQueueKey(queueName));
            var r2 =db.KeyDelete(JobQueueConfig.GetQueueJobIdKey(queueName));
            var r3 =db.KeyDelete(queueName);

        }

        public static async Task Run()
        {

            
            var queueScript = File.ReadAllText("queueJob.lua");

            var redis = ConnectionMultiplexer.Connect("192.168.99.100:32768");
            //clear the queues and the job increment
            Cleanup(redis);

            
            //var queueKeys = new RedisKey[] { "queue:1" };


            //var stopwatch =  Stopwatch.StartNew();
            //var threads = new List<Task>();
            //for (int i = 0; i < 4; i++)
            //{
            //    threads.Add(Task.Run(() =>
            //    {
            //        Console.WriteLine("Started Thread");
            //        TaskToRun().Wait();
            //    }));
            //}

            //await Task.WhenAll(threads).ConfigureAwait(false);
            //Console.WriteLine($"finished in {(float)stopwatch.ElapsedMilliseconds / 1000f}s");



            //passing the job id is not the final solution - calc incrby insert the job when queueing the job on the redis cluster
            //return the queued job id back to the client


            await Task.Delay(5000);


        }

        public static async Task<IEnumerable<Job>> GetDequeuedJob()
        {
            var dequeuekeys = new RedisKey[] { "queue:1", "inprogressqueue:1" };
            var db = Redis.GetDatabase();

            var dequeueResult = await db.ScriptEvaluateAsync(DequeueScript, dequeuekeys).ConfigureAwait(false);

            if (dequeueResult.IsNull)
            {
                return Enumerable.Empty<Job>();
            }

            return new[] {JsonConvert.DeserializeObject<Job>(dequeueResult.ToString())};
        }

        public static async Task TaskToRun()
        {
            var db = Redis.GetDatabase();
            long jobId = 0;
            var numberOfJobsToAdd = 10000;
            for (int j = 0; j < 20; j++)
            {
                var stopwatch = Stopwatch.StartNew();
                for (int i = 0; i < numberOfJobsToAdd; i++)
                {
                    var payload = "12344444";
                    var job = new Job() {JobId = jobId, Payload = payload };
                    //var job = $"{{'JobId':'{jobId}','Payload':'{payload}'}}";

                    var result = await db.ListRightPushAsync("queue:1", JsonConvert.SerializeObject(job)).ConfigureAwait(false);
                    //var poped = await db.ListLeftPopAsync("queue:1");
                    var dequeuedJob = await GetDequeuedJob().ConfigureAwait(false);
                    //var queueResult = await db.ScriptEvaluateAsync(queueScript, queueKeys, new RedisValue[] { job }).ConfigureAwait(false);
                    //Console.WriteLine(queueResult);
                }
                //Console.WriteLine();
                stopwatch.Stop();
                var count = await db.HashLengthAsync("inprogressqueue:1").ConfigureAwait(false);
                Console.WriteLine($"{count} in the inprogress queue");
                Console.WriteLine($"{jobId} : Time taken to queue {numberOfJobsToAdd} jobs : {stopwatch.ElapsedMilliseconds}ms");
            }
        }

        public static async Task StatusPoller()
        {
            while (true)
            {
                await Task.Delay(500);
                
            }
            
        }
    }
}