namespace IOServices.Api.Output;

public enum ResourceType
{
    None,
    Kafka,
    File,
    Combined
}

public interface IResource //todo refactor
{
    ResourceType Type { get; }
}

public class CombinedResource : IResource
{
    public ResourceType Type => ResourceType.Combined;
}

public class FileResource : IResource
{
    public ResourceType Type => ResourceType.File;
    public string? Path;

    public FileResource(string? path)
    {
        Path = path;
    }
}

public class KafkaResource : IResource
{
    public ResourceType Type => ResourceType.Kafka;
    public string? Topic;

    public KafkaResource(string? topic)
    {
        Topic = topic;
    }
}