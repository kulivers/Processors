using System.Collections.Concurrent;
using PlatformEntities.Platform.BrokerMessage.Contracts;

namespace PlatformEntities.Platform.BrokerMessage.Entities
{
    public class ListenerService : IListenerService, IDisposable
    {
        private ConcurrentDictionary<string, QueueListener> _listeners = new ConcurrentDictionary<string, QueueListener>();

        public Result<SubscriptionResult> Listen(IMessageBrokerConsumer consumer, IQueueKey key, MessageReceivedHandler handler)
        {
            try
            {
                string id = consumer.ConsumerKey.Id;
                var listener = _listeners.GetOrAdd(id, _ => new QueueListener(consumer, key));

                listener.Add(key, handler);
                return new SubscriptionResult() { IsSubscribed = true };
            }
            catch (Exception ex)
            {
                return new Result<SubscriptionResult>() { Exception = ex };
            }
        }

        public Result<SubscriptionResult> Forget(IMessageBrokerConsumer consumer, IQueueKey key)
        {
            try
            {
                string id = consumer.ConsumerKey.Id;
                if (_listeners.TryRemove(id, out var listener))
                {
                    listener.Remove(key);
                    listener.Dispose();
                }
                return new SubscriptionResult() { IsSubscribed = false };
            }
            catch (Exception ex)
            {
                return new Result<SubscriptionResult>() { Exception = ex };
            }
        }

        /// <inheritdoc/>
        public TWaitType WaitForReply<TWaitType>(ConcurrentDictionary<string, TWaitType> replies, string replyKey, TimeSpan timeout) where TWaitType : class
        {
            var tokenSource = new CancellationTokenSource(timeout);
            var token = tokenSource.Token;
            TWaitType reply = null;
            try
            {
                var task = Task.Run(async () =>
                {
                    while (true)
                    {
                        reply = replies[replyKey];

                        if (reply != null)
                        {
                            break;
                        }
                        token.ThrowIfCancellationRequested();
                        await Task.Delay(100, token);
                    }
                }, token);
                task.Wait(token);
            }
            catch (OperationCanceledException)
            {}
            catch (AggregateException ae)
            {
                if (!(ae.InnerException is TaskCanceledException))
                {
                    throw ae.InnerException;
                }
            }
            finally
            {
                replies.TryRemove(replyKey, out _);
            }
            return reply;
        }

        public void Dispose()
        {
            foreach (var i in _listeners)
            {
                i.Value.Dispose();
            }
            _listeners = null;
        }
    }
}
