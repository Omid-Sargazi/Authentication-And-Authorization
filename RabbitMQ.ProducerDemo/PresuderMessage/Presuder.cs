using System.Reflection.Metadata;
using System.Text;
using RabbitMQ.Client;

namespace RabbitMQ.ProducerDemo.PresuderMessage
{
    public class Producer
    {
        public static void Run()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "admin",
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            string exchange = "main-exchange";
            string routingKey = "main-queue";


            var props = channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object>
            {
                { "x-retry", 0 }
            };



            // channel.QueueDeclare(queue: "my-queue",
            //     durable: false,
            //     exclusive: false,
            //     autoDelete: false,
            //     arguments: null
            // );

            string message = "error:needs retry error";
            var body = Encoding.UTF8.GetBytes(message);


            channel.BasicPublish(exchange,
                routingKey,
                basicProperties: props,
                body: body
            );

            Console.WriteLine($"[x] Sent: {message}");
            
        }
    }
}