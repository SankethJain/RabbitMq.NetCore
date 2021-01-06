using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMq.NetCore.NewTask
{
    // https://github.com/rabbitmq/rabbitmq-tutorials/blob/master/dotnet/NewTask/NewTask.cs
    class Program
    {
        /// <summary>
        /// Work queue
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            // Queues are idempotent, i.e, it will only be created if it doesn't exist already
            channel.QueueDeclare(queue: "task_queue",
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            string message = GetMessage(args);
            var body = Encoding.UTF8.GetBytes(message);
            
            // To ensure queue is not lost even if RabbitMQ restarts
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: "", routingKey: "task_queue", basicProperties: properties, body: body);

            Console.WriteLine($" [x] Sent {message}");

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
        }
    }
}
