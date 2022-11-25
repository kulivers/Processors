using Confluent.Kafka;

namespace IOServices.Api.Output.Configs;

public class KafkaOutputConfig : IOutputServiceConfig
{
    public IEnumerable<string> Topics;
    public ProducerConfig Client;
    public ToSendFilterConfig? ToSendFilter;
    public MessageValueType MessageValueType;
    public MessageKeyType MessageKeyType;
}

