using Confluent.Kafka;
using IOServices.Api;
using IOServices.Api.Input;
using IOServices.Api.Input.Configs;
using IOServices.Api.Output;
using IOServices.Api.Output.Configs;
using IOServices.Input.Kafka;
using PlatformEntities.Platform.BrokerMessage;
using PlatformEntities.Platform.BrokerMessage.Entities;

namespace IOServices.Output.Kafka;

public static class KafkaOutputFactory
{
    public static IOutputService Create(KafkaOutputConfig config)
    {
        var serviceType = typeof(KafkaInputT<,>);
        var keyType = GetKeyType(config.MessageKeyType);
        var valueType = GetValueType(config.MessageValueType);
        var genericServiceType = serviceType.MakeGenericType(keyType, valueType);
        
        var keySerializer = GetSerializer(config.MessageKeyType);
        var valueSerializer = GetSerializer(config.MessageValueType);
        return (IOutputService)Activator.CreateInstance(genericServiceType, config, keySerializer, valueSerializer) ?? throw new InvalidOperationException();
    }

    private static Type GetKeyType(MessageKeyType keyType) =>
        keyType switch
        {
            MessageKeyType.Null => typeof(Null),
            MessageKeyType.Int => typeof(int),
            _ => throw new ArgumentOutOfRangeException(nameof(keyType), keyType, null)
        };

    private static Type GetValueType(MessageValueType valueType) =>
        valueType switch
        {
            MessageValueType.BrokerMessage => typeof(BrokerMessage),
            MessageValueType.String => typeof(string),
            _ => throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null)
        };

    private static object GetSerializer(MessageValueType valueType)
    {
        return valueType switch
        {
            MessageValueType.BrokerMessage => new BrokerMessageSerializerDeserializer(),
            MessageValueType.String => new StringSerializerDerializer(),
            _ => throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null)
        };
    }

    private static object? GetSerializer(MessageKeyType keyType)
    {
        return keyType switch
        {
            MessageKeyType.Null => null,
            MessageKeyType.Int => null,
            _ => throw new ArgumentOutOfRangeException(nameof(keyType), keyType, null)
        };
    }
}

