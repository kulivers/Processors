using PlatformEntities;

namespace Processors.Api;

public interface IProcessor
{
    string Name { get; set;  }
    void CheckHealth();
}

public interface IProcessor<TIn, TOut> : IProcessor
{
    public ProcessorResult<TOut> Process(TIn value, CancellationToken token);
    public Task<ProcessorResult<TOut>> ProcessAsync(TIn value, CancellationToken token);
}