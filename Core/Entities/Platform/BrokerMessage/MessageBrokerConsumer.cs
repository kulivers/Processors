using System.Collections.Concurrent;
using Confluent.Kafka;
using PlatformEntities.Platform.BrokerMessage.Contracts;
using PlatformEntities.Platform.BrokerMessage.Entities;

namespace PlatformEntities.Platform.BrokerMessage
{
    /// <summary>
    /// This class represents an implementation of <see cref="IMessageBrokerConsumer"/> based on the Apache Kafka.
    /// </summary>
    public class MessageBrokerConsumer : IMessageBrokerConsumer, IDisposable
    {
        #region Fields

        private readonly string bootstrapServers;
        private readonly string groupId;
        private readonly ConcurrentDictionary<IQueueKey, IConsumer<Null, Entities.BrokerMessage>> subscriptions;
        private readonly BrokerMessageSerializerDeserializer _serializerDeserializer;
        private readonly ConsumerBuilder<Null, Entities.BrokerMessage> consumerBuilder;
        private readonly IProducer<Null, Entities.BrokerMessage> producer;
        private readonly ConsumerConfig consumerConfig;
        private bool disposed;

        #endregion

        #region Ctors

        /// <summary>
        /// Creates a new instance of the <see cref="MessageBrokerConsumer"/>.
        /// </summary>
        /// <param name="messageBrokerFactory">Message broker factory.</param>
        /// <param name="consumerKey">The <see cref="IConsumerKey"/></param>
        public MessageBrokerConsumer(IMessageBrokerFactory messageBrokerFactory, IConsumerKey consumerKey)
        {
            var config = messageBrokerFactory.GetConfig().Value;
            this.bootstrapServers = config.BootstrapServers;
            this.groupId = groupId = config.GroupId;
            this.ConsumerKey = consumerKey;


            consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = this.bootstrapServers,
                GroupId = this.groupId,
                AutoOffsetReset = AutoOffsetReset.Latest,
                FetchMaxBytes = 1024 * 1024 * 100,
                MaxPartitionFetchBytes = 1024 * 1024 * 100,
            };

            ProducerConfig producerConfig = new ProducerConfig()
            {
                BootstrapServers = this.bootstrapServers,
                MessageMaxBytes = 1024 * 1024 * 100,
            };

            _serializerDeserializer = new BrokerMessageSerializerDeserializer();
            this.consumerBuilder = new ConsumerBuilder<Null, Entities.BrokerMessage>(consumerConfig).SetValueDeserializer(_serializerDeserializer);
            this.producer = new ProducerBuilder<Null, Entities.BrokerMessage>(producerConfig).SetValueSerializer(_serializerDeserializer).Build();
            this.subscriptions = new ConcurrentDictionary<IQueueKey, IConsumer<Null, Entities.BrokerMessage>>();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="MessageBrokerConsumer"/>.
        /// </summary>
        /// <param name="messageBrokerFactory">Message broker factory.</param>
        public MessageBrokerConsumer(IMessageBrokerFactory messageBrokerFactory) : this(messageBrokerFactory, new ConsumerKey())
        { }

        #endregion

        #region Properties

        /// <inheritdoc />
        public IConsumerKey ConsumerKey { get; }

        public IQueueKey QueueKey { get; }
        #endregion

        #region Members
        /// <inheritdoc />
        public Result<AcknowledgmentResult> Ack(IQueueKey queueKey, IMessageKey messageKey)
        {
            if (!this.subscriptions.TryGetValue(queueKey, out var consumer))
            {
                return new Result<AcknowledgmentResult>() { Exception = new Exception("The consumer is not subscribed to the queue.") };
            }
            return new Result<AcknowledgmentResult>(new AcknowledgmentResult() { IsAcknowledged = true, MessageKey = messageKey, QueueKey = queueKey });
        }

        /// <inheritdoc />
        public Result<Entities.BrokerMessage> Consume(IQueueKey queueKey)
        {
            if (!this.subscriptions.TryGetValue(queueKey, out var consumer))
            {
                return new Result<Entities.BrokerMessage>() { Exception = new Exception("The consumer is not subscribed to the queue.") };
            }

            try
            {
                var consumeResult = consumer.Consume(10);
                if (consumeResult?.Message?.Value != null)
                {
                    return new Result<Entities.BrokerMessage>(consumeResult.Message.Value);
                }
                else
                {
                    return new Result<Entities.BrokerMessage>(null);
                }
            }
            catch (ThreadAbortException tae)
            {
                throw tae;
            }
            catch (ConsumeException e)
            {
                return new Result<Entities.BrokerMessage>(null);
            }
        }

        /// <inheritdoc />
        public bool Equals(IMessageBrokerConsumer other)
        {
            if (other is null)
            {
                return false;
            }

            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.GetType() != other.GetType())
            {
                return false;
            }

            return this.ConsumerKey == other.ConsumerKey;
        }

        /// <inheritdoc />
        public Result<PublishingResult> Publish(IQueueKey queueKey, Dictionary<string, object> messagePayload, PublishingDelegate onPublishing = null)
        {
            return this.Publish(queueKey, null, messagePayload, onPublishing);
        }

        /// <inheritdoc />
        public Result<PublishingResult> Publish(IQueueKey queueKey, IMessageKey replyOn, Dictionary<string, object> messagePayload, PublishingDelegate onPublishing = null)
        {
            MessageKey messageKey = new MessageKey(MessageBrokerIdProvider.GetStringId());
            Entities.BrokerMessage message = new Entities.BrokerMessage()
            {
                ReplyOn = replyOn,
                MessageKey = messageKey,
                QueueKey = queueKey,

                PublisherKey = this.ConsumerKey,
                PublishedDateTime = DateTime.Now,
                Data = messagePayload
            };

            onPublishing?.Invoke(message);
            this.producer.Produce(queueKey.QueueName, new Message<Null, Entities.BrokerMessage>() { Value = message }, (r) =>
            {
                
            });
            return new Result<PublishingResult>(new PublishingResult() { IsPublished = true, MessageKey = messageKey, Publisher = this.ConsumerKey, QueueKey = queueKey });
        }

        /// <inheritdoc />
        public Result<RestoringResult> Restore(IQueueKey queueKey, IMessageKey messageKey)
        {
            return new Result<RestoringResult>(new RestoringResult() { IsRestored = true, MessageKey = messageKey, QueueKey = queueKey });
        }

        /// <inheritdoc />
        public Result<SubscriptionResult> Subscribe(IQueueKey queueKey)
        {
            var consumer = this.subscriptions.GetOrAdd(queueKey, (key) =>
                {
                    var c = consumerBuilder.Build();
                    return c;
                });
            consumer.Subscribe(queueKey.QueueName);
            return new Result<SubscriptionResult>(new SubscriptionResult() { IsSubscribed = true, QueueKey = queueKey });
        }

        /// <inheritdoc />
        public Result<SubscriptionResult> Unsubscribe(IQueueKey queueKey)
        {
            if (this.subscriptions.TryRemove(queueKey, out var consumer))
            {
                consumer.Unsubscribe();
                consumer.Dispose();
            }
            return new Result<SubscriptionResult>(new SubscriptionResult() { IsSubscribed = false, QueueKey = queueKey });
        }

        #endregion

        #region Implementation of IDisposable


        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc cref="Dispose()" />
        ~MessageBrokerConsumer()
        {
            this.Dispose(false);
        }

        /// <inheritdoc cref="Dispose()" />
        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }
            this.disposed = true;


            try
            { }
            finally
            {
                this.producer?.Dispose();
                foreach (var entry in this.subscriptions)
                {
                    if (entry.Value != null)
                    {
                        entry.Value.Close();
                        entry.Value.Unsubscribe();
                        entry.Value.Dispose();
                    }
                }
            }
        }

        #endregion
    }
}
