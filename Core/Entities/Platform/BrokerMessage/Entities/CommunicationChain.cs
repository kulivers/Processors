using PlatformEntities.Platform.BrokerMessage.Contracts;

namespace PlatformEntities.Platform.BrokerMessage.Entities
{
    /// <summary>
    /// This class represents a communication chain.
    /// </summary>
    public class CommunicationChain : ICommunicationChain
    {
        #region Properties
        /// <inheritdoc />
        public IQueueKey RequestKey { get; }
        /// <inheritdoc />
        public IQueueKey ReplyKey { get; }
        /// <inheritdoc />
        public IQueueKey ErrorKey { get; }
        /// <inheritdoc />
        public IQueueKey StatusKey { get; }
        #endregion

        #region Ctors
        /// <summary>
        /// Creates an instance of the <see cref="CommunicationChain"/>.
        /// </summary>
        /// <param name="requestKey">The request key.</param>
        /// <param name="replyKey">The response key.</param>
        public CommunicationChain(string requestKey, string replyKey)
        {
            if (string.IsNullOrEmpty(requestKey))
            {
                throw new ArgumentNullException(nameof(requestKey));
            }
            if (string.IsNullOrEmpty(replyKey))
            {
                throw new ArgumentNullException(nameof(replyKey));
            }
            this.RequestKey = new QueueKey(requestKey);
            this.ReplyKey = new QueueKey(replyKey);
        }

        public CommunicationChain(string requestKey, string replyKey, string errorKey, string statusKey)
            : this(requestKey, replyKey)
        {
            if (string.IsNullOrEmpty(errorKey))
            {
                throw new ArgumentNullException(nameof(errorKey));
            }
            if (string.IsNullOrEmpty(statusKey))
            {
                throw new ArgumentNullException(nameof(statusKey));
            }
            this.ErrorKey = new QueueKey(errorKey);
            this.StatusKey = new QueueKey(statusKey);
        }

        /// <summary>
        /// Creates an instance of the <see cref="CommunicationChain"/>.
        /// </summary>
        /// <param name="requestKey">The request key.</param>
        /// <param name="replyKey">The response key.</param>
        public CommunicationChain(IQueueKey requestKey, IQueueKey replyKey)
        {
            this.RequestKey = requestKey ?? throw new ArgumentNullException(nameof(requestKey));
            this.ReplyKey = replyKey ?? throw new ArgumentNullException(nameof(replyKey));
        }

        public CommunicationChain(IQueueKey requestKey, IQueueKey replyKey, IQueueKey errorKey, IQueueKey statusKey)
            : this(requestKey, replyKey)
        {
            this.ErrorKey = errorKey ?? throw new ArgumentNullException(nameof(errorKey));
            this.StatusKey = statusKey ?? throw new ArgumentNullException(nameof(statusKey));
        }

        public CommunicationChain(string consumerId)
        {
            this.RequestKey = new QueueKey($"request_queue_{consumerId}");
            this.ReplyKey = new QueueKey($"reply_queue_{consumerId}");
        }
        #endregion
    }
}