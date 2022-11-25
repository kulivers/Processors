using PlatformEntities.Platform.BrokerMessage.Contracts;

namespace PlatformEntities.Platform.BrokerMessage.Entities
{
    /// <summary>
    /// This class represents a message key.
    /// </summary>
    public class MessageKey : IMessageKey, IEquatable<IMessageKey>
    {
        #region Ctors

        /// <summary>
        /// Creates a new instance of the <see cref="MessageKey"/>.
        /// </summary>
        /// <param name="key">The key</param>
        public MessageKey(string key)
        {
            this.MessageId = key;
        }

        /// <summary>
        /// Dont use !. This constructor is for Json. Creates a new instance of the <see cref="MessageKey"/>.
        /// </summary>
        public MessageKey()
        { }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public string MessageId { get; set; }

        /// <inheritdoc/>
        public string Tag { get; set; }

        /// <inheritdoc />
        public bool Equals(IMessageKey other)
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

            return (MessageId == other.MessageId);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.MessageId.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj) => this.Equals(obj as IMessageKey);

        #endregion
    }
}

