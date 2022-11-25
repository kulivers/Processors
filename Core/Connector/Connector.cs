using System.Diagnostics;
using Confluent.Kafka;
using Connector.Api;
using IOServices.Api.Input;
using IOServices.Api.Input.Configs;
using IOServices.Api.Output;
using IOServices.Input.Kafka;
using Newtonsoft.Json;
using PlatformEntities;
using PlatformEntities.Platform.BrokerMessage.Entities;
using Processors.Api;
using ProcessorsContainer;
using ProxyTypeMapper;

namespace Connector;

public class Connector : IConnector
{
    public IInputService InputService { get; }
    public IOutputService? OutputService { get; }
    public IProcessor Processor { get; }
    private const double SecondsToResponse = 0.1; //todo to config
    private const double SecondsToCheckHealth = 5;
    private IProxyTypeMapper MessageAdapter { get; }

    internal Connector(IInputService inputService, IProcessor processor, Api.ProxyTypeMapper proxyTypeMapper, IOutputService? outputService = null)
    {
        MessageAdapter = ProxyTypeMapperFactory.Create(proxyTypeMapper);
        InputService = inputService;
        InputService.OnReceive += (_, inputServiceMessage) =>
        {
            var jsonData = MessageAdapter.Map(inputServiceMessage);
            var token = new CancellationTokenSource(TimeSpan.FromSeconds(SecondsToResponse)).Token; //todo to cfg
            var processorResponse = (IProcessorResult)ProcessorCaller.Process(processor, jsonData, token)!;
            OnProcessorResponse(inputServiceMessage, jsonData, processorResponse);
        };

        Processor = processor;
        OutputService = outputService;
    }

    private void OnProcessorResponse(IInputServiceMessage inputServiceMessage, string processorInput, IProcessorResult processorResult)
    {
        InputService.OnResponse(inputServiceMessage, processorResult);
        OutputService?.Send(inputServiceMessage, processorInput, processorResult);
    }

    public async Task StartReceive(CancellationToken token)
    {
        await InputService.StartReceive(token);
    }

    public void CheckHealth()
    {
        InputService.CheckHealth(SecondsToCheckHealth);
        Processor.CheckHealth();
        OutputService?.CheckHealth(SecondsToCheckHealth);
    }
}