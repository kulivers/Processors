using PlatformEntities.Platform.BrokerMessage.Contracts;

namespace PlatformEntities.Platform.BrokerMessage.Entities
{
    /// <summary>
    /// This class represents a FS based consumer key.
    /// </summary>
    public class ConsumerKey : IConsumerKey
    {
        #region Ctors
        /// <summary>
        /// Creates a new instance of the <see cref="ConsumerKey"/>.
        /// </summary>
        /// <param name="key"></param>
        public ConsumerKey(string key)
        {
            this.Id = key;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ConsumerKey"/>.
        /// </summary>
        public ConsumerKey()
        { }
        #endregion

        #region Properties
        /// <inheritdoc/>
        public string Id { get; set; }

        #endregion    
    }
}

