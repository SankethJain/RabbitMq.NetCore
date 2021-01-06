using RabbitMQ.Client;
using System;
using System.Text;

namespace RabbitMq.NetCore.Send
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            //var factory = new ConnectionFactory()
            //{
            //    HostName = "something.mq.ap-south-1.amazonaws.com",
            //    UserName = "sanketh",
            //    Password = "",
            //    Port = 5671,
            //    RequestedHeartbeat = TimeSpan.FromSeconds(60),
            //    Ssl =
            //        {
            //            ServerName = "something.mq.ap-south-1.amazonaws.com",
            //            Enabled = true
            //        }
            //};
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            // Queues are idempotent, i.e, it will only be created if it doesn't exist already
            channel.QueueDeclare(queue: "hello",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            string message = "Hello World";
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);

            Console.WriteLine($" [x] Sent {message}");

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
