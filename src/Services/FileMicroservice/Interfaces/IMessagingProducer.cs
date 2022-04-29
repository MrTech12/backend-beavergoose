namespace FileMicroservice.Interfaces
{
    public interface IMessagingProducer
    {
        void SendMessage<T>(T message, string routingKey);
    }
}
