using Confluent.Kafka;
using Newtonsoft.Json;
using PlatformEntities.Platform.BrokerMessage.Entities;
using PlatformEntities.Platform.BrokerMessage.utils;

namespace PlatformEntities.Platform.BrokerMessage
{
    /// <summary>
    /// This class represents a <see cref="BrokerMessage"/> serializer
    /// </summary>
    public class BrokerMessageSerializerDeserializer : ISerializer<Entities.BrokerMessage>, IDeserializer<Entities.BrokerMessage>
    {
        private static readonly Newtonsoft.Json.JsonSerializer serializer = Newtonsoft.Json.JsonSerializer.Create(new Newtonsoft.Json.JsonSerializerSettings() { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto, ReferenceLoopHandling = ReferenceLoopHandling.Ignore, Error = OnSerializationError });
        private static void OnSerializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e)
        {
            e.ErrorContext.Handled = true;
        }

        /// <inheritdoc />
        public Entities.BrokerMessage Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            if (isNull)
            {
                return null;
            }
            return data.ToArray().Deserialize<Entities.BrokerMessage>();
        }

        /// <inheritdoc />
        public byte[] Serialize(Entities.BrokerMessage data, SerializationContext context)
        {
            return data.Serialize(false);
        }
    }
}
