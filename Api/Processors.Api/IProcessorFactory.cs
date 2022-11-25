namespace Processors.Api;

public interface IProcessorFactory<TIn, TOut>
{
    IProcessor GetOrCreateProcessor(string dllPath, string configPath, string processorName);
}