namespace PlatformEntities.Platform.BrokerMessage.Contracts
{
    public interface IConsumerProvider
    {
        IMessageBrokerConsumer GetConsumer(IConsumerKey key);
    }
}