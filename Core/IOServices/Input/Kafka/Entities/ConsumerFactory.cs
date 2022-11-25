using Confluent.Kafka;
using PlatformEntities.Platform.BrokerMessage;
using PlatformEntities.Platform.BrokerMessage.Entities;

namespace IOServices.Input.Kafka.Entities
{
    public class ConsumerFactoryGeneric<K, V>
    {
        private ConsumerConfig ConsumerConfig { get; }

        public ConsumerFactoryGeneric(ConsumerConfig consumerConfig)
        {
            ConsumerConfig = consumerConfig;
        }


        public IConsumer<K, V> CreateConsumer()
        {
            return new ConsumerBuilder<K, V>(ConsumerConfig).Build();
        }

        public IConsumer<K, V> CreateConsumer(IDeserializer<V> deserializer)
        {
            var consumerBuilder = new ConsumerBuilder<K, V>(ConsumerConfig).SetValueDeserializer(deserializer);
            return consumerBuilder.Build();
        }

        public IConsumer<K, V> CreateConsumer(IDeserializer<K> deserializer)
        {
            var consumerBuilder = new ConsumerBuilder<K, V>(ConsumerConfig).SetKeyDeserializer(deserializer);
            return consumerBuilder.Build();
        }

        public IConsumer<K, V> CreateConsumer(IDeserializer<K>? keyDeserializer, IDeserializer<V>? valueDeserializer)
        {
            if (keyDeserializer == null && valueDeserializer == null)
            {
                return CreateConsumer();
            }

            if (keyDeserializer == null && valueDeserializer != null)
            {
                return CreateConsumer(valueDeserializer);
            }

            if (keyDeserializer != null && valueDeserializer == null)
            {
                return CreateConsumer(keyDeserializer);
            }

            var consumerBuilder = new ConsumerBuilder<K, V>(ConsumerConfig).SetKeyDeserializer(keyDeserializer).SetValueDeserializer(valueDeserializer);
            return consumerBuilder.Build();
        }
    }

    public class ConsumerFactory
    {
        private ConsumerConfig ConsumerConfig { get; }

        public ConsumerFactory(ConsumerConfig consumerConfig)
        {
            ConsumerConfig = consumerConfig;
        }


        public IConsumer<Null, BrokerMessage> CreateBrokerMessageConsumer()
        {
            var serializer = new BrokerMessageSerializerDeserializer();
            var consumerBuilder = new ConsumerBuilder<Null, BrokerMessage>(ConsumerConfig).SetValueDeserializer(serializer);
            return consumerBuilder.Build();
        }

        public IConsumer<int, BrokerMessage> CreateIntBrokerMessageConsumer()
        {
            var serializer = new BrokerMessageSerializerDeserializer();
            var consumerBuilder = new ConsumerBuilder<int, BrokerMessage>(ConsumerConfig).SetValueDeserializer(serializer);
            return consumerBuilder.Build();
        }

        public IConsumer<int, string> CreateStringConsumer()
        {
            var builder = new ConsumerBuilder<int, string>(ConsumerConfig);
            var deserializer = new StringDeserializer();
            return builder.SetValueDeserializer(deserializer).Build();
        }
    }
}