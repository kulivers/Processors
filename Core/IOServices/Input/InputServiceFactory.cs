using IOServices.Api;
using IOServices.Api.Input;
using IOServices.Api.Input.Configs;
using IOServices.Input.Kafka;
using IOServices.Input.Memory;

namespace IOServices.Input;

public static class InputServiceFactory
{
    public static IInputService Create(IInputServiceConfig config)
    {
        return config switch
        {
            KafkaInputConfig kafkaConfig => Create(kafkaConfig),
            MemoryInputConfig memoryConfig => Create(memoryConfig),
            _ => throw new NotImplementedException()
        };
    }

    public static IInputService Create(KafkaInputConfig config)
    {
        //return new KafkaInput(config);
        return KafkaInputFactory.Create(config);
    }

    public static IInputService Create(MemoryInputConfig config)
    {
        return new MemoryInput(config.DirsToWatch);
    }
}