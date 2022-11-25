using System.Collections.Concurrent;
using PlatformEntities.Platform.BrokerMessage.Contracts;

namespace PlatformEntities.Platform.BrokerMessage.Entities
{
    internal class QueueListener : IDisposable
    {
        #region fields
        private readonly TimeSpan _pollingInterval = TimeSpan.FromMilliseconds(100);

        private IMessageBrokerConsumer _consumer;
        private Dictionary<string, IQueueKey> _queues = new Dictionary<string, IQueueKey>();
        private Dictionary<string, MessageReceivedHandler> _handlers = new Dictionary<string, MessageReceivedHandler>();
        private Task _listenTask;
        private CancellationTokenSource _taskCts = new CancellationTokenSource();

        private ConcurrentBag<IQueueKey> _added = new ConcurrentBag<IQueueKey>();
        private ConcurrentBag<IQueueKey> _removed = new ConcurrentBag<IQueueKey>();

        private bool _disposed = false;
        #endregion

        #region ctors
        public QueueListener(IMessageBrokerConsumer consumer, IQueueKey key)
        {
            _consumer = consumer;
            _consumer.Subscribe(key);
            _queues = new Dictionary<string, IQueueKey>();
            _listenTask = Task.Factory.StartNew(ListenLoop, _taskCts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
        #endregion

        #region methods
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                _taskCts.Cancel();
                _listenTask.Wait();
            }
            catch
            {
                // ignored
            }
            _taskCts.Dispose();
            _listenTask.Dispose();

            _disposed = true;
        }

        ~QueueListener()
        {
            Dispose();
        }

        public void Add(IQueueKey queueKey, MessageReceivedHandler handler)
        {
            _handlers[queueKey.QueueName] = handler;
            _added.Add(queueKey);
        }

        public void Remove(IQueueKey queueKey)
        {
            _removed.Add(queueKey);
        }

        private void ListenLoop()
        {
            while (true)
            {
                bool changed = !_added.IsEmpty || !_removed.IsEmpty;

                while (_added.TryTake(out var queue))
                {
                    _queues[queue.QueueName] = queue;
                }

                while (_removed.TryTake(out var queue))
                {
                    if (!_queues.Remove(queue.QueueName))
                    {
                        _removed.Add(queue);
                    }
                }

                if (_queues.Count == 0)
                {
                    if (changed)
                    {
                        return;
                    }
                    else
                    {
                        Thread.Sleep(_pollingInterval);
                    }
                }

                foreach (var i in _queues)
                {
                    if (this._taskCts.IsCancellationRequested)
                    {
                        return;
                    }

                    var result = _consumer.Consume(i.Value);

                    if (result?.Value != null)
                    {
                        try
                        {
                            _handlers[i.Key].Invoke(result.Value);
                        }
                        catch (Exception e)
                        {
                        }
                        _consumer.Ack(i.Value, result.Value.MessageKey);
                    }
                    else
                    {
                        Thread.Sleep(_pollingInterval);
                    }
                }
            }
        }

        #endregion
    }
}