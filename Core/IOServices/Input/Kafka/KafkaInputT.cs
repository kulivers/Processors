using Confluent.Kafka;
using IOServices.Api.Input;
using IOServices.Api.Input.Configs;
using IOServices.Input.Kafka.Entities;
using IOServices.Output.Kafka.Exceptions;
using PlatformEntities;
using PlatformEntities.Platform.BrokerMessage.Entities;

namespace IOServices.Input.Kafka;

public class KafkaInputT<K, V> : IInputService<ConsumeResult<K, V>>
{
    private readonly IEnumerable<string> _inputTopics;

    private readonly ConsumerConfig _consumerConfig;
    public IConsumer<K, V> BrokerMessageConsumer { get; set; }

    public event EventHandler<IInputServiceMessage>? OnReceive;
    public event EventHandler<IInputServiceMessage<ConsumeResult<K, V>>>? OnReceiveT;

    private readonly bool _commitOnError;

    public KafkaInputT(KafkaInputConfig config, IDeserializer<K>? keyDeserializer, IDeserializer<V>? valueDeserializer)
    {
        _commitOnError = config.CommitOnError;
        _inputTopics = config.Topics;
        _consumerConfig = config.Client;
        BrokerMessageConsumer = new ConsumerFactoryGeneric<K, V>(_consumerConfig).CreateConsumer(keyDeserializer, valueDeserializer);
    }

    public async Task StartReceive(CancellationToken token)
    {
        BrokerMessageConsumer.Subscribe(_inputTopics);

        while (!token.IsCancellationRequested)
        {
            try
            {
                var received = BrokerMessageConsumer.Consume(token);
                if (received == null)
                {
                    continue;
                }

                CallOnMessageEvent(new InputServiceMessage<ConsumeResult<K, V>>(received, null));
            }

            catch (ConsumeException e)
            {
                if (e.InnerException is ArgumentException)
                {
                    //message key is not K type, we ignore it
                }
                else
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }

    private protected virtual void CallOnMessageEvent(IInputServiceMessage<ConsumeResult<K, V>> recieved)
    {
        OnReceive?.Invoke(this, recieved);
        OnReceiveT?.Invoke(this, recieved);
    }

    public void CheckHealth(double secondsToResponse)
    {
        var adminConfig = new AdminClientConfig(_consumerConfig);
        var adminClient = new AdminClientBuilder(adminConfig).Build();
        using (adminClient)
        {
            foreach (var topic in _inputTopics)
            {
                var metadata = adminClient.GetMetadata(topic, TimeSpan.FromSeconds(secondsToResponse));
                foreach (var topicMetadata in metadata.Topics)
                {
                    if (topicMetadata.Error.IsError)
                    {
                        throw new TopicNotAvailableException(topicMetadata.Topic, topicMetadata.Error.Reason);
                    }
                }
            }
        }
    }

    public void OnResponse(IInputServiceMessage input, IProcessorResult processorResult)
    {
        OnResponse((InputServiceMessage<ConsumeResult<K, V>>)input, processorResult);
    }

    public void OnResponse(IInputServiceMessage<ConsumeResult<K, V>> input, IProcessorResult processorResult)
    {
        if (_commitOnError || (processorResult.Success))
        {
            BrokerMessageConsumer.Commit(input.Data);
        }
    }
}