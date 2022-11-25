using System.Text;
using Confluent.Kafka;

namespace IOServices.Output.Kafka.Entities;

public class StringSerializer : ISerializer<string?>
{
    public byte[] Serialize(string? data, SerializationContext context)
    {
        return data != null ? Encoding.UTF8.GetBytes(data) : new byte[] { 0 };
    }
}