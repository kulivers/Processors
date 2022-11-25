using PlatformEntities.Platform.BrokerMessage.Contracts;

namespace PlatformEntities.Platform.BrokerMessage.Entities
{
    /// <summary>
    /// This class represnets <see cref="IMessageBroker"/>'s routing key.
    /// </summary>
    public class RoutingKey : IRoutingKey
    {
        #region Ctors
        /// <summary>
        /// Creates an instance of the <see cref="RoutingKey"/> using specified settings.
        /// </summary>
        /// <param name="chains">The communication chains.</param>
        public RoutingKey(IEnumerable<(string requestKey, string replyKey, string errorKey, string statusKey)> chains) 
            : this(chains == null 
                  ? null 
                  : new List<ICommunicationChain>(chains.Select(it => new CommunicationChain(it.requestKey, it.replyKey, it.errorKey, it.statusKey))))
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="RoutingKey"/> using specified settings.
        /// </summary>
        /// <param name="chains">The communication chains.</param>
        public RoutingKey(IEnumerable<ICommunicationChain> chains)
        {
            if (chains == null)
            {
                throw new ArgumentNullException(nameof(chains));
            }

            this.Chains = chains;
            this.DefaultChain = chains.First();
        }
        #endregion

        #region Properties
        /// <inheritdoc />
        public IEnumerable<ICommunicationChain> Chains { get; set; }

        /// <inheritdoc />
        public ICommunicationChain DefaultChain { get; }
        #endregion    
    }
}

