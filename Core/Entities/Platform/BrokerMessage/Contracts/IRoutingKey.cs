namespace PlatformEntities.Platform.BrokerMessage.Contracts
{
    /// <summary>
    /// This interface represnets an abstract <see cref="IMessageBroker"/>'s routing key.
    /// </summary>
    public interface IRoutingKey
    {
        /// <summary>
        /// Gets linked <see cref="ICommunicationChain"/> items.
        /// </summary>
        IEnumerable<ICommunicationChain> Chains { get; set; }

        /// <summary>
        /// Gets the default communication chan.
        /// </summary>
        ICommunicationChain DefaultChain { get; }
    }

    /// <summary>
    /// This interface represents a communication chain.
    /// </summary>
    public interface ICommunicationChain
    {
        /// <summary>
        /// Gets or sets the request key.
        /// </summary>
        IQueueKey RequestKey { get; }

        /// <summary>
        /// Gets or sets the response key.
        /// </summary>
        IQueueKey ReplyKey { get; }

        IQueueKey ErrorKey { get; }

        IQueueKey StatusKey { get; }
    }
}
