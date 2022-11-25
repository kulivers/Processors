using System.Collections.Concurrent;
using PlatformEntities.Platform.BrokerMessage.Entities;

namespace PlatformEntities.Platform.BrokerMessage.Contracts
{
    /// <summary>
    /// This delegate represents a handler that responses for the massage handling process.
    /// </summary>
    /// <param name="message">The message that should be processed.</param>
    public delegate void MessageReceivedHandler(Entities.BrokerMessage message);

    /// <summary>
    /// This interface represents an abstract <see cref="IListenerService"/> that responses for messages routing.
    /// </summary>
    public interface IListenerService : IDisposable
    {
        /// <summary>
        /// Listen to the specified <see cref="IQueueKey"/>.
        /// </summary>
        /// <param name="consumer">The <see cref="IMessageBrokerConsumer"/></param>
        /// <param name="queueKey">The <see cref="IQueueKey"/></param>
        /// <param name="handler">The <see cref="MessageReceivedHandler"/>.</param>
        /// <returns></returns>
        Result<SubscriptionResult> Listen(IMessageBrokerConsumer consumer, IQueueKey queueKey, MessageReceivedHandler handler);

        /// <summary>
        /// Forgets the specified <see cref="IMessageBrokerConsumer"/> associated with the <see cref="IQueueKey"/>.
        /// </summary>
        /// <param name="consumer"></param>
        /// <param name="queueKey"></param>
        /// <returns></returns>
        Result<SubscriptionResult> Forget(IMessageBrokerConsumer consumer, IQueueKey queueKey);

        /// <summary>
        /// Waits for a reply on specified request.
        /// </summary>
        /// <typeparam name="TWaitType"></typeparam>
        /// <param name="replies"></param>
        /// <param name="replyKey"></param>
        /// <param name="timeout"></param>
        /// <returns>Returns awaiting result</returns>
        TWaitType WaitForReply<TWaitType>(ConcurrentDictionary<string, TWaitType> replies, string replyKey, TimeSpan timeout) where TWaitType : class;
    }
}