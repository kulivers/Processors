using Connector;
using Connector.Api;

namespace SuperAgent;

public class Agent
{
    public List<IConnector> Connectors { get; }

    public Agent(AgentConfig agentConfig)
    {
        Connectors = new List<IConnector>();
        var connectorConfigs = agentConfig.ConnectorsConfigs;
        Connectors = InitConnectors(connectorConfigs).ToList();
        CheckHealth(Connectors);
    }

    private IEnumerable<IConnector> InitConnectors(IEnumerable<ConnectorConfig> connectorsConfigs)
    {
        var connectors = new List<IConnector>();
        foreach (var connectorConfig in connectorsConfigs)
        {
            var connectorsInternal = ConnectorFactory.CreateConnector(connectorConfig).ToArray();
            connectors.AddRange(connectorsInternal);
        }
        return connectors;
    }


    public void Start()
    {
        var tasks = Connectors.Select(connector => Task.Run(() => connector.StartReceive(CancellationToken.None))).ToArray();//todo cts
        Task.WaitAll(tasks);
    }

    private void CheckHealth(List<IConnector> connectors)
    {
        foreach (var connector in connectors)
        {
            connector.CheckHealth();
        }
    }
}