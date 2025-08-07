using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitMQ.RetryDemo.RetryQueueExample
{
    public class RetryQueue
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

            string mainQueue = "main-queue";
            string retryQueue = "retry-queue";
            string dlqQueue = "main-queue-dlq";

            string mainExchange = "main-exchange";
            string dlxExchange = "dlx-exchange";
            string retryExchange = "retry-exchange";

            channel.ExchangeDeclare(exchange: dlxExchange, type: ExchangeType.Direct);
            channel.QueueDeclare(queue: dlqQueue, durable: false, exclusive: false, autoDelete: false);
            channel.QueueBind(queue: dlqQueue, exchange: dlxExchange, routingKey: dlqQueue);

            var retryArgs = new Dictionary<string, object>
            {
                {"x-dead-letter-exchange",mainExchange},
                {"x-dead-letter-routing-key",mainQueue},
                {"x-message-ttl",5000}
            };

            channel.ExchangeDeclare(exchange: retryExchange, type: ExchangeType.Direct);
            channel.QueueDeclare(queue: retryQueue, durable: false, exclusive: false, autoDelete: false,arguments:retryArgs);
            channel.QueueBind(queue: retryQueue, exchange: retryExchange, routingKey: retryQueue);

            var mainArgs = new Dictionary<string, object>
            {
                {"x-dead-letter-exchange",retryExchange},
                {"x-dead-letter-routing-key",retryQueue}
            };


            channel.ExchangeDeclare(exchange: mainExchange, type: ExchangeType.Direct);
            channel.QueueDeclare(mainQueue, durable: false, exclusive: false, autoDelete: false, arguments: mainArgs);
            channel.QueueBind(queue: mainQueue, exchange: mainExchange, routingKey: mainQueue);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received: " + message);

                try
                {
                    if (message.Contains("error"))
                        throw new Exception("Simulated failure");

                    Console.WriteLine(" ✅ Message processed");
                    channel.BasicAck(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {

                    Console.WriteLine(" ❌ Error: " + ex.Message);
                    bool isFinalRetry = ea.Redelivered;

                    if (isFinalRetry)
                    {
                        channel.BasicReject(ea.DeliveryTag, requeue: false);
                    }
                    else
                    {
                        channel.BasicReject(ea.DeliveryTag, requeue: false);
                    }

                }
            };

            channel.BasicConsume(mainQueue, autoAck: false, consumer: consumer);


            Console.WriteLine(" [*] Waiting for messages...");
            Console.ReadLine();


        }
    }
}