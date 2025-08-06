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

            channel.QueueDeclare(queue: "my-queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            string message = "Hello RabbitMQ";
            var body = Encoding.UTF8.GetBytes(message);


            channel.BasicPublish(exchange: "",
                routingKey: "my-queue",
                basicProperties: null,
                body: body
            );

            Console.WriteLine($"[x] Sent: {message}");
            
        }
    }
}