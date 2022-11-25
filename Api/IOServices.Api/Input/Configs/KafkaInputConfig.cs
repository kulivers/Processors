using Confluent.Kafka;
using IOServices.Api.Output.Configs;

namespace IOServices.Api.Input.Configs;

public class KafkaInputConfig : IInputServiceConfig
{
    public IEnumerable<string> Topics;
    public ConsumerConfig Client;
    public bool CommitOnError;
    public MessageValueType MessageValueType;
    public MessageKeyType MessageKeyType;
}

