using PlatformEntities.Platform.BrokerMessage.Contracts;

namespace PlatformEntities.Platform.BrokerMessage.Entities
{
    /// <summary>
    /// This class represents a <see cref="IMessageBroker"/>'s message.
    /// </summary>
    public class BrokerMessage
    {
        /// <summary>
        /// The <see cref="IMessageKey"/> on that this message is replied.
        /// </summary>
        public IMessageKey ReplyOn { get; set; }
        /// <summary>
        /// Gets or sets the message key.
        /// </summary>
        public IMessageKey MessageKey { get; set; }
        /// <summary>
        /// Gets or sets the publisher key.
        /// </summary>
        public IConsumerKey PublisherKey { get; set; }
        /// <summary>
        /// Gets or sets the queue key.
        /// </summary>
        public IQueueKey QueueKey { get; set; }
        /// <summary>
        /// Gets or sets the publishing date time.
        /// </summary>
        public DateTime PublishedDateTime { get; set; }
        /// <summary>
        /// Gets or sets the messages payload.
        /// </summary>
        public Dictionary<string, object> Data { get; set; }
    }
}
