using FileMicroservice.Helpers;
using FileMicroservice.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace FileMicroservice.Messaging
{
    public class RabbitMQProducer : IMessagingProducer
    {
        private readonly ILogger<RabbitMQProducer> _logger;
        private readonly IConfiguration _configuration;
        private const string exchangeName = "link-exchange";
        private const string queueName = "link.managing";

        public RabbitMQProducer(IConfiguration configuration, ILogger<RabbitMQProducer> logger)
        {
            this._configuration = configuration;
            this._logger = logger;
        }

        public void SendMessage<T>(T message, string routingKey)
        {
            this._logger.LogInformation("Sending message {message}", message);

            var factory = new ConnectionFactory()
            {
                HostName = RetrieveConfigHelper.GetConfigValue("RabbitMQ", "HostName"),
                Port = Convert.ToInt32(RetrieveConfigHelper.GetConfigValue("RabbitMQ", "Port")),
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);
                channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey, arguments: null);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true; // Declaring the message as persistent
                channel.BasicQos(0, 1, false); // Send messages to different workers based on received acknowledgment

                string messageBody = JsonConvert.SerializeObject(message);
                byte[] body = Encoding.UTF8.GetBytes(messageBody);

                channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: null, body: body);
            }
        }
    }
}
