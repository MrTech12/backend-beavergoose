using Common.Configuration.Helpers;
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
        private readonly ILogger _logger;
        private const string exchangeName = "link-exchange";
        private const string queueName = "link.managing";

        public RabbitMQProducer(ILogger<RabbitMQProducer> logger)
        {
            this._logger = logger;
        }

        public void SendMessage<T>(T message, string routingKey)
        {
            this._logger.LogInformation("Sending status of a file to routingkey: {RoutingKey}", routingKey);

            var factory = new ConnectionFactory()
            {
                HostName = ConfigHelper.GetConfigValue("RabbitMQ", "HostName"),
                Port = Convert.ToInt32(ConfigHelper.GetConfigValue("RabbitMQ", "Port")),
            };

            try
            {
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
            catch (Exception exception)
            {
                this._logger.LogError(exception, "There was a problem connection or communicating with RabbitMQ. Source of Exception: {Source}. Expection Message: {Message}", exception.Source, exception.Message);
                throw;
            }
        }
    }
}
