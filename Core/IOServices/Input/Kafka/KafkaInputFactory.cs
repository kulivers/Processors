using System.Text;
using Confluent.Kafka;
using IOServices.Api;
using IOServices.Api.Input;
using IOServices.Api.Input.Configs;
using PlatformEntities.Platform.BrokerMessage;
using PlatformEntities.Platform.BrokerMessage.Entities;

namespace IOServices.Input.Kafka;

public static class KafkaInputFactory
{
    public static IInputService Create(KafkaInputConfig config)
    {
        var serviceType = typeof(KafkaInputT<,>);
        var keyType = GetKeyType(config.MessageKeyType);
        var valueType = GetValueType(config.MessageValueType);
        var genericServiceType = serviceType.MakeGenericType(keyType, valueType);
        
        var keySerializer = GetDeserializer(config.MessageKeyType);
        var valueSerializer = GetDeserializer(config.MessageValueType);
        return (IInputService)Activator.CreateInstance(genericServiceType, config, keySerializer, valueSerializer) ?? throw new InvalidOperationException();
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

    private static object GetDeserializer(MessageValueType valueType)
    {
        return valueType switch
        {
            MessageValueType.BrokerMessage => new BrokerMessageSerializerDeserializer(),
            MessageValueType.String => new StringSerializerDerializer(),
            _ => throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null)
        };
    }

    private static object? GetDeserializer(MessageKeyType keyType)
    {
        return keyType switch
        {
            MessageKeyType.Null => null,
            MessageKeyType.Int => null,
            _ => throw new ArgumentOutOfRangeException(nameof(keyType), keyType, null)
        };
    }
}

