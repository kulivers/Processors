using Confluent.Kafka;
using Connector.Api;
using ElasticClient.Entities;
using IOServices.Api.Input;
using Newtonsoft.Json;
using PlatformEntities.Platform.BrokerMessage.Entities;

namespace ProxyTypeMapper;

public static class ProxyTypeMapperFactory
{
    public static IProxyTypeMapper Create(Connector.Api.ProxyTypeMapper type)
    {
        return type switch
        {
            Connector.Api.ProxyTypeMapper.None => new DefaultTypeMapper(),
            Connector.Api.ProxyTypeMapper.KafkaToElastic => new KafkaToElasticTypeMapper(),
            Connector.Api.ProxyTypeMapper.MemoryToElastic => new MemoryToElasticTypeMapper(),
            _ => throw new NotImplementedException()
        };
    }
}

internal class DefaultTypeMapper : IProxyTypeMapper
{
    public string Map(IInputServiceMessage input)
    {
        return JsonConvert.SerializeObject(input.GetData());
    }
}

internal class MemoryToElasticTypeMapper : IProxyTypeMapper
{
    public string Map(IInputServiceMessage input)
    {
        return input.IsJson ? (string)input.GetData() : throw new NotSupportedException();
    }
}

internal class KafkaToElasticTypeMapper : IProxyTypeMapper
{
    public string Map(IInputServiceMessage input)
    {
        if (input.GetData() is InputServiceMessage<ConsumeResult<Null, string>>)
        {
            return ((InputServiceMessage<ConsumeResult<Null, string>>)input).Data.Message.Value;
        }
        if (input.GetData() is ConsumeResult<Null, string> consumeResultString)
        {
            return consumeResultString.Message.Value;
        }
        BrokerMessage? brokerMessage = input.GetData() switch
        {
            ConsumeResult<Null, BrokerMessage> consumeResult => consumeResult?.Message.Value,
            ConsumeResult<int, BrokerMessage> consumeResult2 => consumeResult2?.Message.Value,
            _ => throw new InvalidCastException("Cannot cast input to ConsumeResult<Null, BrokerMessage> or ConsumeResult<int, BrokerMessage>")
        };

        var inputData = brokerMessage?.Data;
        if (inputData == null)
        {
            return null;
        }

        HttpMethod? method = null;
        var headers = new Dictionary<string, IEnumerable<string>>();
        string? address = null;
        string? data = null;


        if (inputData.TryGetValue("METHOD", out var method1))
        {
            if (method1 is string value)
            {
                method = new HttpMethod(value);
            }
            else
            {
                method = (HttpMethod)method1;
            }
        }

        if (inputData.TryGetValue("Method", out var method2))
        {
            if (method2 is string value)
            {
                method = new HttpMethod(value);
            }
            else
            {
                method = (HttpMethod)method2;
            }
        }

        if (inputData.TryGetValue("method", out var method3))
        {
            if (method3 is string value)
            {
                method = new HttpMethod(value);
            }
            else
            {
                method = (HttpMethod)method3;
            }
        }

        if (inputData.TryGetValue("Headers", out var headers1))
        {
            var hs = (Dictionary<string, IEnumerable<string>>)headers1;
            foreach (var (key, value) in hs)
            {
                headers.Add(key, value);
            }
        }

        if (inputData.TryGetValue("headers", out var headers2))
        {
            var hs = (Dictionary<string, IEnumerable<string>>)headers2;
            foreach (var (key, value) in hs)
            {
                headers.Add(key, value);
            }
        }

        if (inputData.TryGetValue("address", out var address1))
        {
            address = (string)address1;
        }

        if (inputData.TryGetValue("Address", out var address2))
        {
            address = (string)address2;
        }

        if (inputData.TryGetValue("Data", out var payload1))
        {
            data = (string)payload1;
        }

        if (inputData.TryGetValue("data", out var payload2))
        {
            data = (string)payload2;
        }

        if (method == null || address == null)
        {
            throw new ArgumentException("Cannot parse method and address");
        }

        var elasticRequestMessage = new ElasticRequestMessage(method, address, data, headers);
        return JsonConvert.SerializeObject(elasticRequestMessage);
    }

    private static Uri BuildUri(string elasticAddress, string index, string? type = "_doc", string? docId = null)
    {
        return docId != null
            ? new Uri($"{elasticAddress}/{index}/{type}/{docId}")
            : new Uri($"{elasticAddress}/{index}/{type}");
    }
}