namespace PlatformEntities.Platform.BrokerMessage.Contracts
{

    /// <summary>
    /// This interface represents the <see cref="IMessageBroker"/>'s queue key.
    /// </summary>
    public interface IQueueKey
    {
        /// <summary>
        /// Gets the queue name.
        /// </summary>
        string QueueName { get; }
    }

    /// <summary>
    /// This interface represents an abstract message key.
    /// </summary>
    public interface IMessageKey
    {
        /// <summary>
        /// Gets the message identifier.
        /// </summary>
        string MessageId { get; }

        /// <summary>
        /// Gets or sets the message tag.
        /// </summary>
        string Tag { get; }
    }
}
