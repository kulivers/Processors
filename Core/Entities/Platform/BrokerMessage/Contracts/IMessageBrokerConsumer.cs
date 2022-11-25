using PlatformEntities.Platform.BrokerMessage.Entities;

namespace PlatformEntities.Platform.BrokerMessage.Contracts
{
    /// <summary>
    /// This interface represents an abstract <see cref="IMessageBroker"/> consumer.
    /// </summary>
    public interface IMessageBrokerConsumer : IEquatable<IMessageBrokerConsumer>
    {
        /// <summary>
        /// Gets the consumer key.
        /// </summary>
        IConsumerKey ConsumerKey { get; }

        /// <summary>
        /// Publishes the message to the <see cref="IQueueKey"/>.
        /// </summary>
        /// <param name="queueKey">The queue key.</param>
        /// <param name="message">The message payload.</param>
        /// <param name="onPublishing">The <see cref="PublishingDelegate"/>.</param>
        /// <returns>The operation result.</returns>
        Result<PublishingResult> Publish(IQueueKey queueKey, Dictionary<string, object> messagePayload, PublishingDelegate onPublishing = null);

        /// <summary>
        /// Subscribes to the specified <see cref="IQueueKey"/>.
        /// </summary>
        /// <param name="queueKey">The <see cref="IQueueKey"/>.</param>
        /// <returns>The operation result.</returns>
        Result<SubscriptionResult> Subscribe(IQueueKey queueKey);

        /// <summary>
        /// Unsubscribes the <see cref="IMessageBrokerConsumer"/> from the specified <see cref="IQueueKey"/>.
        /// </summary>
        /// <param name="queueKey">The <see cref="IQueueKey"/>.</param>
        /// <returns>The operation result.</returns>
        Result<SubscriptionResult> Unsubscribe(IQueueKey queueKey);

        /// <summary>
        /// Reads next message from the specified <see cref="IQueueKey"/>.
        /// </summary>
        /// <param name="queueKey">The queue key.</param>
        /// <returns>The operation result.</returns>
        Result<Entities.BrokerMessage> Consume(IQueueKey queueKey);

        /// <summary>
        /// Acknowledges the message.
        /// </summary>
        /// <param name="queueKey">The <see cref="IQueueKey"/>.</param>
        /// <param name="messageKey">The <see cref="IMessageKey>."/></param>
        /// <returns>The operation result.</returns>
        Result<AcknowledgmentResult> Ack(IQueueKey queueKey, IMessageKey messageKey);

        /// <summary>
        /// Restores the message.
        /// </summary>
        /// <param name="queueKey">The <see cref="IQueueKey"/>.</param>
        /// <param name="messageKey">The <see cref="IMessageKey>."/></param>
        /// <returns>The operation result.</returns>
        Result<RestoringResult> Restore(IQueueKey queueKey, IMessageKey messageKey);
    }
}
