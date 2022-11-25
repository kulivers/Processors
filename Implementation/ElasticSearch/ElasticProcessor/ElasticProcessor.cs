using ElasticClient;
using ElasticClient.Config;
using ElasticClient.Entities;
using PlatformEntities;
using Processors.Api;

namespace ElasticProcessor;

[ProcessElement(nameof(ElasticProcessor), ProcessingAttributeBehaviourType.Processor)]
public class ElasticProcessor : IProcessor<ElasticRequestMessage, ElasticResponseMessage>
{
    private readonly EsManager _esManager;
    public string Name { get; set; }
    public double SecondsToResponse => 4012;

    private long _count;
    public ElasticProcessor(EsManagerConfig config, string processorName)
    {
        _esManager = new EsManager(config);
        Name = processorName;
        _count = 0;
    }

    public async void CheckHealth()
    {
        try
        {
            var elasticAvailable = _esManager.CheckElasticAvailable(SecondsToResponse);
            await elasticAvailable;
            if (elasticAvailable.Exception != null)
            {
                throw elasticAvailable.Exception;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public ProcessorResult<ElasticResponseMessage> Process(ElasticRequestMessage value, CancellationToken token)
    {
        try
        {
            ElasticResponseMessage responseMessage = _esManager.SendRequest(value, token);
            var index = value.Address.LocalPath.Split('/').FirstOrDefault(s => s != string.Empty);
            Console.WriteLine($"Wrote {value.Method} - {index}. Total messages: {++_count}"); //todo delete
            ProcessorResult<ElasticResponseMessage> output = new ProcessorResult<ElasticResponseMessage>(responseMessage);
            return output;
        }
        catch (TaskCanceledException ex)
        {
            TooBigDelayFromElasticException bigDelayFromElasticException;
            bigDelayFromElasticException = value?.Address?.Host != null
                ? new TooBigDelayFromElasticException(value?.Address?.Host)
                : new TooBigDelayFromElasticException();
            var output = new ProcessorResult<ElasticResponseMessage>(bigDelayFromElasticException);
            return output;
        }
        catch (Exception ex)
        {
            var output = new ProcessorResult<ElasticResponseMessage>(ex);
            return output;
        }
    }

    public async Task<ProcessorResult<ElasticResponseMessage>> ProcessAsync(ElasticRequestMessage value, CancellationToken token)
    {
        try
        {
            var response = await _esManager.SendRequestAsync(value, token);
            var output = new ProcessorResult<ElasticResponseMessage>(response);
            return output;
        }
        catch (TaskCanceledException ex)
        {
            TooBigDelayFromElasticException bigDelayFromElasticException;
            bigDelayFromElasticException = value?.Address?.Host != null
                ? new TooBigDelayFromElasticException(value?.Address?.Host)
                : new TooBigDelayFromElasticException();
            var output = new ProcessorResult<ElasticResponseMessage>(bigDelayFromElasticException);
            return output;
        }
        catch (Exception e)
        {
            var output = new ProcessorResult<ElasticResponseMessage>(e);
            return output;
        }
    }


    public TOut Process<TIn, TOut>(TIn value, CancellationToken token)
    {
        if (value is ElasticRequestMessage esRequest)
        {
            var response = Process(esRequest, token);
            if (response is TOut castedResponse)
                return castedResponse;
        }

        throw new InvalidCastException();
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }
}