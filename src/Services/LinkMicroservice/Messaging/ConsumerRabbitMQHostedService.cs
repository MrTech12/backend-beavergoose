using LinkMicroservice.DTOs;
using LinkMicroservice.Helpers;
using LinkMicroservice.Interfaces;
using LinkMicroservice.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Text;
using System.Text.Json;

namespace LinkMicroservice.Messaging
{
    public class ConsumerRabbitMQHostedService : BackgroundService
    {
        private readonly ILogger<ConsumerRabbitMQHostedService> _logger;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private const string exchangeName = "link-exchange";
        private const string queueName = "link.managing";
        private readonly IConfiguration _configuration;
        private readonly ILinkRepository _linkRepository;
        private readonly RetrieveConfigHelper _retrieveConfigHelper;

        public ConsumerRabbitMQHostedService(IConfiguration configuration, ILogger<ConsumerRabbitMQHostedService> logger, ILinkRepository linkRepository)
        {
            this._configuration = configuration;
            this._logger = logger;
            this._linkRepository = linkRepository;
            this._retrieveConfigHelper = new RetrieveConfigHelper(this._configuration);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var routingKeys = new List<string>()
            {
                "create",
                "delete"
            };

            this._connectionFactory = new ConnectionFactory
            {
                HostName = this._retrieveConfigHelper.GetConfigValue("RabbitMQ", "HostName"),
                Port = Convert.ToInt32(this._retrieveConfigHelper.GetConfigValue("RabbitMQ", "Port")),
                DispatchConsumersAsync = true
            };
            try
            {
                this._connection = _connectionFactory.CreateConnection();
                this._channel = _connection.CreateModel();

                this._channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);
                this._channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                foreach (var routingKey in routingKeys)
                {
                    this._channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);
                }

                var properties = this._channel.CreateBasicProperties();
                properties.Persistent = true; // Declaring the message as persistent
                this._channel.BasicQos(0, 1, false); // Send messages to different workers based on received acknowledgment

                this._logger.LogInformation($"Queue [{queueName}] is waiting for messages.");
            }
            catch (BrokerUnreachableException brokenUnreachable)
            {
                this._logger.LogError("Connection to RabbitMQ can't be established. Exception message: {0}.", brokenUnreachable.Message);
                throw;
            }
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new AsyncEventingBasicConsumer(this._channel);
            consumer.Received += async (model, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                this._logger.LogInformation($"Message received with content: [{content}].");
                var fileDto = JsonSerializer.Deserialize<FileDTO>(content);

                if (fileDto != null)
                {
                    await HandleMessage(fileDto, ea.RoutingKey);
                }

                // Manual acknowledgments do not remove message from queue so automatic for the time being.
                //this._channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false); // Letting RabbitMQ know that the message had been received.
            };
            this._channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

            await Task.CompletedTask;
        }

        public async Task HandleMessage(FileDTO fileDto, string routingKey)
        {
            if (routingKey == "create")
            {
                var linkService = new LinkService(this._linkRepository);
                await linkService.CreateSaveLink(fileDto);
            }
            else if (routingKey == "delete")
            {
                var linkService = new LinkService(this._linkRepository);
                await linkService.RemoveLink(fileDto);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            this._connection.Close();
            this._logger.LogInformation("RabbitMQ connection is closed.");
        }
    }
}
