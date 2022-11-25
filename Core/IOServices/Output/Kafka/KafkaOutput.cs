using Confluent.Kafka;
using IOServices.Api.Input;
using IOServices.Api.Output;
using IOServices.Api.Output.Configs;
using IOServices.Output.Kafka.Entities;
using IOServices.Output.Kafka.Exceptions;
using PlatformEntities;

namespace IOServices.Output.Kafka;

public class KafkaOutput : IOutputService
{
    private readonly ProducerConfig _producerConfig;
    private readonly IOutputServiceFilter? _filter;
    private IProducer<Null, string?> StringProducer { get; }
    private IEnumerable<string> OutputTopics { get; }

    public KafkaOutput(KafkaOutputConfig kafkaOutputConfig, IOutputServiceFilter? filter = null) : this(new ProducerConfig(kafkaOutputConfig.Client),
        kafkaOutputConfig.Topics, filter)
    {
    }

    public KafkaOutput(ProducerConfig producerConfig, IEnumerable<string> outputTopics, IOutputServiceFilter? filter = null)
    {
        _producerConfig = producerConfig;
        StringProducer = new ProducerFactory(_producerConfig).CreateStringProducer();
        OutputTopics = outputTopics;
        _filter = filter;
    }

    public OutputResponseModel Send(IInputServiceMessage inputServiceMessage, string processorInput, IProcessorResult processorResult)
    {
        if (_filter != null && !_filter.ShouldSend(processorResult))
        {
            return new OutputResponseModel(new KafkaResource(null), null, false);
        }

        return SendString(processorInput);
    }

    public async Task<OutputResponseModel> SendAsync(IInputServiceMessage inputServiceMessage, string processorInput, IProcessorResult processorResult,
        CancellationToken token)
    {
        if (_filter != null && !_filter.ShouldSend(processorResult))
        {
            return new OutputResponseModel(new KafkaResource(null), null, false);
        }

        return await SendStringAsync(processorInput, token);
    }

    private async Task<OutputResponseModel> SendStringAsync(string? toSend, CancellationToken token)
    {
        var message = new Message<Null, string?>() { Value = toSend };
        var outputModels = new List<OutputResponseModel>();

        foreach (var topic in OutputTopics)
        {
            try
            {
                var deliveryResult = await StringProducer.ProduceAsync(topic, message, token);
                var outputModel = new OutputResponseModel(new KafkaResource(topic), deliveryResult, true);
                outputModels.Add(outputModel);
            }
            catch (Exception e)
            {
                var outputModel = new OutputResponseModel(new KafkaResource(topic), e);
                outputModels.Add(outputModel);
            }
        }

        return new OutputResponseModel(outputModels);
    }

    private OutputResponseModel SendString(string toSend)
    {
        var message = new Message<Null, string?>() { Value = toSend };
        var outputModels = new List<OutputResponseModel>();

        foreach (var topic in OutputTopics)
        {
            try
            {
                throw new Exception(); //todo fix
                StringProducer.Produce(topic, message);
                outputModels.Add(new OutputResponseModel(new KafkaResource(topic), null, true));
            }
            catch (Exception e)
            {
                var outputModel = new OutputResponseModel(new KafkaResource(topic), e);
                outputModels.Add(outputModel);
            }
        }

        return new OutputResponseModel(outputModels);
    }

    public void CheckHealth(double secondsToResponse)
    {
        var adminConfig = new AdminClientConfig(_producerConfig);
        var adminClient = new AdminClientBuilder(adminConfig).Build();
        using (adminClient)
        {
            foreach (var outputTopic in OutputTopics)
            {
                var metadata = adminClient.GetMetadata(outputTopic, TimeSpan.FromSeconds(secondsToResponse));
                foreach (var topic in metadata.Topics)
                {
                    if (topic.Error.IsError)
                    {
                        throw new TopicNotAvailableException(topic.Topic, topic.Error.Reason);
                    }
                }
            }
        }
    }
}