using IOServices.Api;
using IOServices.Api.Input;
using IOServices.Api.Input.Configs;
using IOServices.Api.Output;
using IOServices.Api.Output.Configs;
using Processors.Api;

namespace Connector.Api;

public class ConnectorConfig
{
    public IEnumerable<ProcessorConfig> Processors { get; set; }
    
    public InputServiceType InputType { get; set; }
    public string InputServiceConfigPath { get; set; }
    public IInputServiceConfig InputConfig { get; set; }
    
    public OutputServiceType? OutputType { get; set; }
    public string? OutputServiceConfigPath { get; set; }
    public IOutputServiceConfig? OutputConfig { get; set; }
    public ProxyTypeMapper ProxyTypeMapper { get; set; }
}


public enum ProxyTypeMapper
{
    None,
    KafkaToElastic,
    MemoryToElastic
}