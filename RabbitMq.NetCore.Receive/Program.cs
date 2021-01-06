using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMq.NetCore.Receive
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

            // Queue created in consumer also, in case it starts before the producer
            // This way, we can be sure that there is a queue to read from, even if it is empty
            // Queues are idempotent, i.e, it will only be created if it doesn't exist already
            channel.QueueDeclare(queue: "hello",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine(" [x] Received {0}", message);
            };

            channel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);
            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
