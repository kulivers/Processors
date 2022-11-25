using ElasticClient.Config;
using ElasticClient.Entities;
using Processors.Api;

namespace ElasticProcessor;

[ProcessElement(nameof(ElasticProcessorFactory), ProcessingAttributeBehaviourType.Factory)]
public class ElasticProcessorFactory : IProcessorFactory<ElasticRequestMessage, ElasticResponseMessage>
{
    private HashSet<IProcessor> Processors { get; }

    public ElasticProcessorFactory()
    {
        Processors = new HashSet<IProcessor>();
    }

    public IProcessor GetOrCreateProcessor(string dllPath, string configPath, string processorName) 
    {
        if (dllPath == null || configPath == null || processorName == null)
        {
            throw new ArgumentException("need to specify all arguments");
        }

        var esClientConfig = EsManagerConfig.FromYaml(configPath);
        var elasticProcessor = new ElasticProcessor(esClientConfig, processorName);
        Processors.Add(elasticProcessor);
        return elasticProcessor;
    }
}