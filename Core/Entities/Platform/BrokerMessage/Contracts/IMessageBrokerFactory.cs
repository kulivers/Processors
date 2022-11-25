using PlatformEntities.Platform.BrokerMessage.Entities;

namespace PlatformEntities.Platform.BrokerMessage.Contracts
{
    /// <summary>
    /// This interface represents a factory for message broker API staff.
    /// </summary>
    public interface IMessageBrokerFactory
    {
        /// <summary>
        /// Creates a <see cref="IRoutingKey"/>.
        /// </summary>
        /// <param name="communicationChains">The <see cref="ICommunicationChain"/> array</param>
        /// <returns></returns>
        Result<IRoutingKey> CreateRoutingKey(ICommunicationChain[] communicationChains);

        /// <summary>
        /// Creates a new <see cref="IQueueKey"/>.
        /// </summary>
        /// <param name="queueName">The queue name</param>
        /// <returns>The <see cref="IQueueKey"/></returns>
        Result<IQueueKey> CreateQueueKey(string queueName);

        /// <summary>
        /// Create a new <see cref="IConsumerKey"/>
        /// </summary>
        /// <param name="consumerKey">The <see cref="IConsumerKey"/></param>
        /// <returns></returns>
        Result<IConsumerKey> CreateConsumerKey(string consumerKey);

        Result<MessageBrokerConfig> GetConfig();
    }

    public class MessageBrokerConfig
    {
        public string BootstrapServers { get; set; }
        public string GroupId { get; set; }

        public MessageBrokerConfig Copy()
        {
            return new MessageBrokerConfig { BootstrapServers = BootstrapServers, GroupId = GroupId };
        }
    }
}
