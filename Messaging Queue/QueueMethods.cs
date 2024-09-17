using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace MarketPlace_API_Gateway.Messaging_Queue
{
    public class QueueMethods : IQueueMethods
    {
        private IModel _channel;

        public QueueMethods()
        {
            var factory = new ConnectionFactory
            {
                HostName = Environment.GetEnvironmentVariable("MessagingQueueHostName"),
                UserName = Environment.GetEnvironmentVariable("MessagingQueueUsername"),
                Password = Environment.GetEnvironmentVariable("MessagingQueuePassword")
            };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            _channel.QueueDeclare(
                queue: "Inventory_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            _channel.QueueDeclare(
                queue: "ShoppingCart_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        }

        public void SendTask(string action, string requiredInfo, string toService)
        {
            var body = Encoding.UTF8.GetBytes(requiredInfo);
            IBasicProperties props = _channel.CreateBasicProperties();
            props.ContentType = "application/json"; //btw this does nothing, cause rabbitmq sends it as a byte array
            props.DeliveryMode = 2;
            props.Headers = new Dictionary<string, object> { { "Action", action } };
            _channel.BasicPublish(
                exchange: string.Empty,
                routingKey: toService + "_queue",
                basicProperties: props,
                body: body
            );
        }
    }
}
