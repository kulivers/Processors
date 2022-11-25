using IOServices.Api.Output;
using IOServices.Api.Output.Configs;
using IOServices.Output.Combined;
using IOServices.Output.Kafka;
using IOServices.Output.Memory;

namespace IOServices.Output;

public static class OutPutServiceFactory
{
    public static IOutputService Create(IOutputServiceConfig? config)
    {
        switch (config)
        {
            case KafkaOutputConfig kafkaConfig:
            {
                if (kafkaConfig.ToSendFilter == null)
                {
                    return Create(kafkaConfig);
                }

                var filter = new ToSendFilter(kafkaConfig.ToSendFilter);
                return Create(kafkaConfig, filter);
            }
            case MemoryOutputConfig memoryOutputConfig:
            {
                if (memoryOutputConfig.ToSendFilter == null)
                {
                    return Create(memoryOutputConfig);
                }
                var filter = new ToSendFilter(memoryOutputConfig.ToSendFilter);
                return Create(memoryOutputConfig, filter);
            }
            case CombinedOutputConfig combinedOutputConfig:
            {
                var services = new IOutputService[combinedOutputConfig.OutputServiceConfigs.Length];
                for (var i = 0; i < combinedOutputConfig.OutputServiceConfigs.Length; i++)
                {
                    var serviceConfig = combinedOutputConfig.OutputServiceConfigs[i];
                    services[i] = Create(serviceConfig);
                }
                
                var mode = combinedOutputConfig.Mode;
                return new CombinedOutput(mode, services);
            }
            default:
                throw new NotImplementedException();
        }
    }

    public static IOutputService Create(KafkaOutputConfig config, IOutputServiceFilter? filter = null)
    {
        return new KafkaOutput(config, filter);
    }

    public static IOutputService Create(MemoryOutputConfig config, IOutputServiceFilter? filter = null)
    {
        return new MemoryOutput(config, filter);
    }
}