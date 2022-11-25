using Localization.IOServices;

namespace IOServices.Output.Kafka.Exceptions;

internal class TopicNotAvailableException : Exception
{
    private static readonly string TopicNotAvailable = IOServicesRecources.TopicNotAvailable;
    
    public TopicNotAvailableException(string topic, string reason) : base(string.Format(TopicNotAvailable, topic, reason))
    {
        
        
    }
}