using Connector.Api;
using IOServices.Api.Input;
using IOServices.Api.Output;
using IOServices.Input;
using IOServices.Output;
using Processors.Api;
using ProcessorsContainer;

namespace Connector;

public static class ConnectorFactory
{
    public static IEnumerable<IConnector> CreateConnector(ConnectorConfig config)
    {
        var processorConfigs = config.Processors;
        var inputConfig = config.InputConfig;
        var outputConfig = config.OutputConfig;
        var proxyTypeMapper = config.ProxyTypeMapper;
        return processorConfigs.Select(pc => CreateConnector(pc, inputConfig, proxyTypeMapper, outputConfig));
    }

    public static IConnector CreateConnector(ProcessorConfig processorConfig, IInputServiceConfig inputServiceConfig,
        Api.ProxyTypeMapper proxyTypeMapper, IOutputServiceConfig? outputServiceConfig = null)
    {
        var processor = ProcessorFactory.CreateProcessor(processorConfig.Dll, processorConfig.Config, processorConfig.Name);
        var inputService = InputServiceFactory.Create(inputServiceConfig);
        var outputService = OutPutServiceFactory.Create(outputServiceConfig);
        var connector = CreateConnector(inputService, processor, proxyTypeMapper, outputService);
        return connector;
    }

    public static IConnector CreateConnector(IInputService inputService, IProcessor processor, Api.ProxyTypeMapper proxyTypeMapper,
        IOutputService? outputService = null)
    {
        //it will not be simple
        var connector = new Connector(inputService, processor, proxyTypeMapper, outputService);
        return connector;
    }
}