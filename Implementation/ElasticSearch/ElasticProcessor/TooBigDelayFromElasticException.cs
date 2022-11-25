using Localization.Libs;

namespace ElasticProcessor;

public class TooBigDelayFromElasticException : Exception
{
    private static readonly string TooBigDelayFromElastic = ElasticClientResources.TooBigDelayFromElastic;
    public TooBigDelayFromElasticException() : base(string.Format(TooBigDelayFromElastic, null!))
    {
        
    }
    public TooBigDelayFromElasticException(string host) : base(string.Format(TooBigDelayFromElastic, host))
    {
        
    }
}