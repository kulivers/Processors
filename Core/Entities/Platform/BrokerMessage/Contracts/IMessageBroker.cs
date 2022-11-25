using PlatformEntities.Platform.BrokerMessage.Entities;

namespace PlatformEntities.Platform.BrokerMessage.Contracts
{
    /// <summary>
    /// Occurs when a <see cref="BrokerMessage"/> is publishing.
    /// </summary>
    /// <param name="brokerMessage"></param>
    public delegate void PublishingDelegate(Entities.BrokerMessage brokerMessage);

    /// <summary>
    /// This interface represents an abstract message broker.
    /// </summary>
    public interface IMessageBroker
    {
        /// <summary>
        /// Subscribes the <see cref="IConsumerKey"/> to the specified <see cref="IQueueKey"/>.
        /// </summary>
        /// <param name="consumerKey">The <see cref="IConsumerKey"/> to be subscribed.</param>
        /// <param name="queueKey">The <see cref="IQueueKey"/> for the consumer.</param>
        /// <returns>The operation result.</returns>
        Result<SubscriptionResult> Subscribe(IConsumerKey consumerKey, IQueueKey queueKey);

        /// <summary>
        /// Consumes the next message from the specified <see cref="IQueueKey"/>.
        /// </summary>
        /// <param name="consumerKey">The <see cref="IConsumerKey"/>.</param>
        /// <param name="queueKey">The <see cref="IQueueKey"/>.</param>        
        /// <returns>The operation result.</returns>
        Result<Entities.BrokerMessage> Consume(IConsumerKey consumerKey, IQueueKey queueKey);

        /// <summary>
        /// Unsabscribe the <see cref="IConsumerKey"/> from the specified <see cref="IQueueKey"/>.
        /// </summary>
        /// <param name="consumerKey">The <see cref="IConsumerKey"/> to be unsubscribed.</param>
        /// <param name="queueKey">The queue key.</param>
        /// <returns>The operation result.</returns>
        Result<SubscriptionResult> Unsubscribe(IConsumerKey consumerKey, IQueueKey queueKey);

        /// <summary>
        /// Mark message as acknowledged.
        /// </summary>
        /// <param name="queueKey">The queue key.</param>
        /// <param name="messageKey">The message key.</param>
        /// <param name="consumerKey">The consumer key.</param>
        /// <returns>The operation result.</returns>
        Result<AcknowledgmentResult> Ack(IConsumerKey consumerKey, IQueueKey queueKey, IMessageKey messageKey);

        /// <summary>
        /// Restores the message.
        /// </summary>
        /// <param name="queueKey">The <see cref="IQueueKey"/>.</param>
        /// <param name="messageKey">The <see cref="IMessageKey>."/></param>
        /// /// <param name="consumerKey">The consumer key.</param>
        /// <returns>The operation result.</returns>
        Result<RestoringResult> Restore(IConsumerKey consumerKey, IQueueKey queueKey, IMessageKey messageKey);

        /// <summary>
        /// Publishes a message using specified parameters.
        /// </summary>
        /// <param name="consumerKey">The consumer key.</param>
        /// <param name="queueKey">The queue key.</param>
        /// <param name="messagePayload">The message payload.</param>
        /// <param name="onPublishing">The <see cref="PublishingDelegate"/>.</param>
        /// <returns>The operation result.</returns>
        Result<PublishingResult> Publish(IConsumerKey consumerKey, IQueueKey queueKey, Dictionary<string, object> messagePayload, PublishingDelegate onPublishing = null);

        /// <summary>
        /// Publishes a message using specified parameters.
        /// </summary>
        /// <param name="consumerKey">The consumer key.</param>
        /// <param name="queueKey">The queue key.</param>
        /// <param name="messagePayload">The message payload.</param>
        /// <param name="replyOn">The </param>
        /// <param name="onPublishing">The <see cref="PublishingDelegate"/>.</param>
        /// <returns>The operation result.</returns>
        Result<PublishingResult> Publish(IConsumerKey consumerKey, IQueueKey queueKey, IMessageKey replyOn, Dictionary<string, object> messagePayload, PublishingDelegate onPublishing = null);
    }
}
