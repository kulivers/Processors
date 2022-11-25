using System.Text;
using Confluent.Kafka;

namespace IOServices.Api.Input;
public class StringSerializerDerializer : ISerializer<string>, IDeserializer<string?>
{
    public byte[] Serialize(string data, SerializationContext context)
    {
        return Encoding.UTF8.GetBytes(data);
    }

    public string? Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return isNull ? null : Encoding.UTF8.GetString(data);
    }
}