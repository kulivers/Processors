using PlatformEntities.Platform.BrokerMessage.Contracts;

namespace PlatformEntities.Platform.BrokerMessage.Entities
{
    /// <summary>
    /// This class represents a queue key.
    /// </summary>
    public class QueueKey : IQueueKey, IEquatable<IQueueKey>
    {
        #region Ctors
        /// <summary>
        /// Creates a new instance of the <see cref="QueueKey"/>.
        /// </summary>
        /// <param name="queueName"></param>
        public QueueKey(string queueName)
        {
            this.QueueName = Path.GetFileName(queueName);

            if (string.IsNullOrEmpty(this.QueueName))
            {
                throw new ArgumentException("Invalid queue key", nameof(queueName));
            }
        }

        /// <summary>
        /// Dont use !. This constructor is for Json. Creates a new instance of the <see cref="QueueKey"/>.
        /// </summary>
        public QueueKey()
        { }

        #endregion

        #region Properties
        /// <inheritdoc/>
        public string QueueName { get; set; }

        /// <inheritdoc/>
        public bool Equals(IQueueKey other)
        {
            if (other is null)
            {
                return false;
            }

            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.GetType() != other.GetType())
            {
                return false;
            }

            return (this.QueueName == other.QueueName);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.QueueName.GetHashCode();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => this.Equals(obj as IQueueKey);

        #endregion
    }
}

