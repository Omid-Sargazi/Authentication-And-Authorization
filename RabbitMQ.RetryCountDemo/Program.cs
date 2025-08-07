// See https://aka.ms/new-console-template for more information
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

Console.WriteLine("Hello, World Count Retry!");


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
string retryExchange = "retry-exchange";
string dlxExchange = "dlx-exchange";

//DLQ
channel.ExchangeDeclare(dlxExchange, ExchangeType.Direct);
channel.QueueDeclare(dlqQueue, durable: false, exclusive: false, autoDelete: false);
channel.QueueBind(dlqQueue, dlxExchange, routingKey: dlqQueue);

var retryArgs = new Dictionary<string, object>
{
    { "x-dead-letter-exchange", mainExchange },
    { "x-dead-letter-routing-key", mainQueue },
    { "x-message-ttl", 5000 } // 5 ثانیه delay
};

channel.ExchangeDeclare(retryExchange, ExchangeType.Direct);
channel.QueueDeclare(retryQueue, durable: false, exclusive: false, autoDelete: false, arguments: retryArgs);
channel.QueueBind(retryQueue, retryExchange, routingKey: retryQueue);

var mainArgs = new Dictionary<string, object>
{
    { "x-dead-letter-exchange", retryExchange },
    { "x-dead-letter-routing-key", retryQueue }
};


channel.ExchangeDeclare(mainExchange, ExchangeType.Direct);
channel.QueueDeclare(mainQueue, durable: false, exclusive: false, autoDelete: false, arguments: mainArgs);
channel.QueueBind(mainQueue, mainExchange, routingKey: mainQueue);

var consumer = new EventingBasicConsumer(channel);

consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    var retryCount = 0;

    if (ea.BasicProperties.Headers != null && ea.BasicProperties.Headers.TryGetValue("x-retry", out var retryValue))
    {
        retryCount = Convert.ToInt32(Encoding.UTF8.GetString((byte[])retryValue));
    }

    Console.WriteLine($" [x] Received: {message}, RetryCount: {retryCount}");

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

        if (retryCount >= 3)
        {
            Console.WriteLine(" 🚨 Moved to DLQ");

            var props = channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object>
            {
                { "x-retry", retryCount }
            };

            channel.BasicPublish(
                exchange: dlxExchange,
                routingKey: dlqQueue,
                basicProperties: props,
                body: body
            );

            channel.BasicAck(ea.DeliveryTag, multiple: false); // دیگه نمی‌خوایم requeue بشه
        }
        else
        {
            Console.WriteLine(" 🔁 Send to retry-queue again");

            var props = channel.CreateBasicProperties();
            props.Headers = new Dictionary<string, object>
            {
                { "x-retry", retryCount + 1 }
            };

            channel.BasicPublish(
                exchange: retryExchange,
                routingKey: retryQueue,
                basicProperties: props,
                body: body
            );

            channel.BasicAck(ea.DeliveryTag, multiple: false); // چون خودمون دوباره publish کردیم
        }
    }
};
        
        channel.BasicConsume(queue: mainQueue, autoAck: false, consumer: consumer);
        Console.WriteLine(" [*] Waiting for messages...");
        Console.ReadLine();