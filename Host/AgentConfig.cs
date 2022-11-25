using Connector.Api;

namespace SuperAgent;

public class AgentConfig
{
    public IEnumerable<ConnectorConfig> ConnectorsConfigs { get; set; }
    
}