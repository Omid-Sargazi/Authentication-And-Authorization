using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.ConsumerDemo.Consumers
{
    public class SimpleConsumer
    {
        public static void Run()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "admin"
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "my-queue",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            Console.WriteLine(" [*] Waiting for messages...");
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received: {0}", message);

                try
                {
                    if (message.Contains("error"))
                    {
                        throw new Exception("simulation an error");
                    }
                    Console.WriteLine(" ✅ Message processed.");
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" ❌ Processing failed: " + ex.Message);

                    channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                    
                }
            };

            channel.BasicConsume(queue: "my-queue",
                autoAck: false,
                consumer: consumer
            );

            Console.ReadLine();
        }
    }
}