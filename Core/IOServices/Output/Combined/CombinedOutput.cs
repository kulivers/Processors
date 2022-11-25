using IOServices.Api.Input;
using IOServices.Api.Output;
using IOServices.Api.Output.Configs;
using PlatformEntities;

namespace IOServices.Output.Combined;

public class CombinedOutput : IOutputService
{
    private readonly IOutputService[] _services;
    private CombinedOutputMode _mode;
    private IOutputServiceFilter? _filter;

    public CombinedOutput(CombinedOutputMode mode, IOutputService[] onUnsuccessfulResponseServices, IOutputServiceFilter? filter = null)
    {
        _mode = mode;
        _services = onUnsuccessfulResponseServices.ToArray();
        _filter = filter;
    }

    public OutputResponseModel Send(IInputServiceMessage inputServiceMessage, string processorInput, IProcessorResult processorResult)
    {
        if (_filter != null && !_filter.ShouldSend(processorResult))
        {
            return new OutputResponseModel(new KafkaResource(null), null, false);
        }

        return _mode switch
        {
            CombinedOutputMode.OneByOneAll => SendAllServices(inputServiceMessage, processorInput, processorResult),
            CombinedOutputMode.OneByOneIfNotSuccess => SendOneByOneIfNotSuccess(inputServiceMessage, processorInput, processorResult),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private OutputResponseModel SendAllServices(IInputServiceMessage inputServiceMessage, string processorInput, IProcessorResult processorResult)
    {
        var responseModels = _services.Select(service => service.Send(inputServiceMessage, processorInput, processorResult)).ToList();
        return new OutputResponseModel(responseModels);
    }

    private OutputResponseModel SendOneByOneIfNotSuccess(IInputServiceMessage inputServiceMessage, string processorInput, IProcessorResult processorResult) //todo as strategy
    {
        var responseModels = new List<OutputResponseModel>();
        foreach (var service in _services)
        {
            var response = service.Send(inputServiceMessage, processorInput, processorResult);
            if (response.IsSuccess)
            {
                return response;
            }

            responseModels.Add(response);
        }

        return new OutputResponseModel(responseModels);
    }

    public async Task<OutputResponseModel> SendAsync(IInputServiceMessage inputServiceMessage, string processorInput, IProcessorResult processorResult, CancellationToken token)
    {
        if (_filter != null && !_filter.ShouldSend(processorResult))
        {
            return new OutputResponseModel(new KafkaResource(null), null, false);
        }

        return _mode switch
        {
            CombinedOutputMode.OneByOneAll => await SendAllAsync(inputServiceMessage, processorInput, processorResult,token),
            CombinedOutputMode.OneByOneIfNotSuccess => await SendOneByOneIfNotSuccessAsync(inputServiceMessage, processorInput, processorResult,token),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private async Task<OutputResponseModel> SendAllAsync(IInputServiceMessage inputServiceMessage, string processorInput, IProcessorResult processorResult, CancellationToken token)
    {
        var responseModels = new List<OutputResponseModel>();
        foreach (var service in _services)
        {
            var response = await service.SendAsync(inputServiceMessage, processorInput, processorResult,token);
            responseModels.Add(response);
        }

        return new OutputResponseModel(responseModels);
    }

    private async Task<OutputResponseModel> SendOneByOneIfNotSuccessAsync(IInputServiceMessage inputServiceMessage, string processorInput, IProcessorResult processorResult, CancellationToken token)
    {
        var responseModels = new List<OutputResponseModel>();
        foreach (var service in _services)
        {
            var response = await service.SendAsync(inputServiceMessage, processorInput, processorResult, token);
            if (response.IsSuccess)
            {
                return response;
            }

            responseModels.Add(response);
        }

        return new OutputResponseModel(responseModels);
    }

    public void CheckHealth(double secondsToResponse)
    {
        foreach (var service in _services)
        {
            service.CheckHealth(secondsToResponse);
        }
    }
}