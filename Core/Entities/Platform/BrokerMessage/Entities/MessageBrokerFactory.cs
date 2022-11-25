using PlatformEntities.Platform.BrokerMessage.Contracts;
using YamlDotNet.Serialization.NamingConventions;

namespace PlatformEntities.Platform.BrokerMessage.Entities
{
    /// <summary>
    /// This class represents a factory for message broker API staff.
    /// </summary>
    public class MessageBrokerFactory : IMessageBrokerFactory
    {
        private readonly MessageBrokerConfig _config;

        public MessageBrokerFactory(Dictionary<string, string> config)
        {
            _config = new MessageBrokerConfig();
            if (!config.TryGetValue("BootstrapServers", out var bootstrapServers))
            {
                throw new ArgumentException();
            }

            _config.BootstrapServers = bootstrapServers;

            if (!config.TryGetValue("GroupId", out var groupId))
            {
                throw new ArgumentException();
            }

            _config.GroupId = groupId;
        }

        public MessageBrokerFactory(string configFile)
        {
            if (!File.Exists(configFile))
            {
                throw new FileNotFoundException(configFile);
            }

            using (var stream = File.OpenRead(configFile))
            using (var reader = new StreamReader(stream))
            {
                var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();
                _config = deserializer.Deserialize<MessageBrokerConfig>(reader);
            }

            if (string.IsNullOrEmpty(_config.BootstrapServers))
            {
                throw new ArgumentException();
            }

            if (string.IsNullOrEmpty(_config.GroupId))
            {
                throw new ArgumentException();
            }
        }


        /// <inheritdoc />
        public Result<IConsumerKey> CreateConsumerKey(string consumerKey)
        {
            return new ConsumerKey(consumerKey);
        }

        /// <inheritdoc />
        public Result<IQueueKey> CreateQueueKey(string queueName)
        {
            try
            {
                QueueKey queueKey = new QueueKey(queueName);
                return new Result<IQueueKey>(queueKey);
            }
            catch (Exception e)
            {
                return new Result<IQueueKey>() { Exception = e };
            }
        }

        /// <inheritdoc />
        public Result<IRoutingKey> CreateRoutingKey(ICommunicationChain[] communicationChains)
        {
            try
            {
                return new Result<IRoutingKey>(new RoutingKey(communicationChains));
            }
            catch (Exception e)
            {
                return new Result<IRoutingKey>() { Exception = e };
            }
        }

        public Result<MessageBrokerConfig> GetConfig()
        {
            return new Result<MessageBrokerConfig>(_config.Copy());
        }
    }
}