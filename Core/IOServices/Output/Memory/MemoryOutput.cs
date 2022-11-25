using System.Text;
using IOServices.Api.Input;
using IOServices.Api.Output;
using IOServices.Api.Output.Configs;
using PlatformEntities;

namespace IOServices.Output.Memory;

public class MemoryOutput : IOutputService //todo as strategy, split to Write and if not success strategy
{
    private readonly IOutputServiceFilter? _filter;

    private readonly string _dirToSaveDocs;
    public MemoryOutput(MemoryOutputConfig kafkaOutputConfig, IOutputServiceFilter? filter = null)
    {
        _filter = filter;
        _dirToSaveDocs = kafkaOutputConfig.DirToSaveDocs;
    }

    public OutputResponseModel Send(IInputServiceMessage inputServiceMessage, string processorInput, IProcessorResult processorResult)
    {
        if (_filter != null && !_filter.ShouldSend(processorResult))
        {
            return new OutputResponseModel(new KafkaResource(null), null, false);
        }

        var fileName = $"{Guid.NewGuid()}.json";
        var filePath = Path.Combine(_dirToSaveDocs, fileName);
        try
        {
            WriteToFile(filePath, processorInput);
            return new OutputResponseModel(new FileResource(filePath), processorInput, true);
        }
        catch (Exception e)
        {
            return new OutputResponseModel(new FileResource(filePath), e);
        }
    }

    public async Task<OutputResponseModel> SendAsync(IInputServiceMessage inputServiceMessage, string processorInput, IProcessorResult processorResult, CancellationToken token)
    {
        return Send(inputServiceMessage, processorInput, processorResult);
    }

    public void CheckHealth(double secondsToResponse)
    {
        if (!Directory.Exists(_dirToSaveDocs))
        {
            Directory.CreateDirectory(_dirToSaveDocs);
        }
    }

    public static void WriteToFile(string path, string toWrite)
    {
        using var fileStream = File.Create(path);
        var bytes = Encoding.UTF8.GetBytes(toWrite);
        fileStream.Write(bytes, 0, bytes.Length);
    }
}