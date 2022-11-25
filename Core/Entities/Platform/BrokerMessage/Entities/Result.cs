using PlatformEntities.Platform.BrokerMessage.Contracts;

namespace PlatformEntities.Platform.BrokerMessage.Entities
{
    /// <summary>
    /// This class reprensent an operation result.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T>
    {
        #region Ctors
        /// <summary>
        /// Creates an instance of the <see cref="Result{T}"/>.
        /// </summary>
        public Result() : this(default(T))
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="Result{T}"/> using specified value.
        /// </summary>
        public Result(T value)
        {
            this.Value = value;
            this.Exception = null;
            this.Data = null;
        } 
        #endregion

        #region Properties
        /// <summary>
        /// Gets the result associated data.
        /// </summary>
        public IDictionary<string, object> Data;

        /// <summary>
        /// Gets the result exception.
        /// </summary>
        public Exception Exception;

        /// <summary>
        /// Gets the result value.
        /// </summary>
        public T Value;
        #endregion

        #region Members
        /// <summary>
        /// Implicitly converts an instance of <see cref="Result{T}"/> to <see cref="T"/>.
        /// </summary>
        /// <param name="result"></param>
        public static implicit operator T(Result<T> result)
        {
            return result.Value;
        }

        /// <summary>
        /// Implicitly convers an instance of <see cref="T"/> to the <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Result<T>(T value)
        {
            return new Result<T>(value);
        } 
        #endregion
    }

    /// <summary>
    /// This struct represents a validation result.
    /// </summary>
    public struct ValidationResult
    {
        /// <summary>
        /// Gets the value indicates the whether the result is successful.
        /// </summary>
        public bool IsSuccessful;
    }

    /// <summary>
    /// This struct represents an acknowledgment result.
    /// </summary>
    public struct AcknowledgmentResult
    {
        /// <summary>
        /// The acknowledgment status.
        /// </summary>
        public bool IsAcknowledged;

        /// <summary>
        /// The <see cref="IQueueKey"/>.
        /// </summary>
        public IQueueKey QueueKey;

        /// <summary>
        /// The message key.
        /// </summary>
        public IMessageKey MessageKey;
    }


    /// <summary>
    /// This struct represents an restoring message result.
    /// </summary>
    public struct RestoringResult
    {
        /// <summary>
        /// The message restoring status.
        /// </summary>
        public bool IsRestored;

        /// <summary>
        /// The <see cref="IQueueKey"/>.
        /// </summary>
        public IQueueKey QueueKey;

        /// <summary>
        /// The message key.
        /// </summary>
        public IMessageKey MessageKey;
    }

    /// <summary>
    /// This struct represents a publishing result.
    /// </summary>
    public struct PublishingResult
    {
        /// <summary>
        /// The publishing status.
        /// </summary>
        public bool IsPublished;

        /// <summary>
        /// The <see cref="IQueueKey"/>.
        /// </summary>
        public IQueueKey QueueKey;

        /// <summary>
        /// The message key.
        /// </summary>
        public IMessageKey MessageKey;

        /// <summary>
        /// The publisher.
        /// </summary>
        public IConsumerKey Publisher;

        /// <summary>
        /// The publishing result.
        /// </summary>
        public DeploymentReply Result;
    }

    /// <summary>
    /// This struct represnets a subscribe result.
    /// </summary>
    public struct SubscriptionResult
    {
        /// <summary>
        /// The subscription result.
        /// </summary>
        public bool IsSubscribed;

        /// <summary>
        /// The <see cref="IQueueKey"/>.
        /// </summary>
        public IQueueKey QueueKey;
    }

    /// <summary>
    /// This struct represents a recieving result.
    /// </summary>
    public struct RecievingResult
    {
        /// <summary>
        /// Indicates if message has been recieved and could be ack.
        /// </summary>
        public bool IsRecieved;

        /// <summary>
        /// The <see cref="IQueueKey"/>.
        /// </summary>
        public IQueueKey QueueKey;

        /// <summary>
        /// The message key.
        /// </summary>
        public IMessageKey MessageKey;
    }

    public class DeploymentReply
    {
        public object Message { get; set; }

        public DeploymentReply(object message)
        {
            Message = message;
        }
    }
}
