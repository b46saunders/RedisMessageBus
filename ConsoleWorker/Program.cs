using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RQueue.Client;
using RQueue.Shared;
using RQueue.Worker;
using StackExchange.Redis;

namespace ConsoleWorker
{
    class Program
    {
        static void Main()
        {
            var connection = ConnectionMultiplexer.Connect("192.168.99.100:32768");
            var fireAndForgetQueueName = "tasks";
            var withResultQueueName = "tasksWithResult";


            RunWithResult(connection, withResultQueueName);
            CountItemsOnQueue(connection, withResultQueueName);

            Run(connection, fireAndForgetQueueName);
            CountItemsOnQueue(connection, fireAndForgetQueueName);

            var input = "";

            while (input != "x")
            {
                input = Console.ReadLine();
            }
        }

        public static async Task Run(ConnectionMultiplexer connection,string queueName)
        {
            

            var status = connection.GetStatus();
            var queue = connection.GetQueue(queueName);

            var handler1 = await queue.RegisterHandler<DummyObject, int>(job =>
            {
                var dummyObject = JsonConvert.DeserializeObject<DummyObject>(job.Payload.ToString());
                //Console.WriteLine($"Received dummy object : {dummyObject}");
                return new ScalarWorkerResult<int>(dummyObject.someIdThatWeNeed, 12);
            });

            Console.WriteLine("Press any key to schedule the worker to stop");
            Console.ReadLine();
            Console.WriteLine("Deregistering worker...");
            handler1.Deregister();//TODO - this should await until all work has been finished so the client can safely end
            Console.WriteLine("Worker stopped");
            
        }

        public static async Task RunWithResult(ConnectionMultiplexer connection, string queueName)
        {
            var status = connection.GetStatus();
            var queue = connection.GetQueue(queueName);

            var handler1 = await queue.RegisterHandler<DummyObject, int>(job =>
            {
                var dummyObject = JsonConvert.DeserializeObject<DummyObject>(job.Payload.ToString());
                //Console.WriteLine($"Received dummy object : {dummyObject}");
                return new ScalarWorkerResult<int>(dummyObject.someIdThatWeNeed, 12);
            });

            Console.WriteLine("Press any key to schedule the worker to stop");
            Console.ReadLine();
            Console.WriteLine("Deregistering worker...");
            handler1.Deregister();//TODO - this should await until all work has been finished so the client can safely end
            Console.WriteLine("Worker stopped");

        }


        public static async Task CountItemsOnQueue(ConnectionMultiplexer connection,string queueName)
        {
            do
            {
                var result = await connection.GetDatabase().ListLengthAsync(queueName);
                Console.WriteLine($"Jobs in queue : {queueName} : {result}");
                await Task.Delay(100);
            } while (true);
            
        }
    }
}