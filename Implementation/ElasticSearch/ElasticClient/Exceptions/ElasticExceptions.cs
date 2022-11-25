using Localization.Libs;

namespace ElasticClient.Exceptions;

public class ElasticSearchNotHealthyException : Exception
{
    private static readonly string ElasticSearchNotHealthy = ElasticClientResources.ElasticSearchNotHealthy;

    public ElasticSearchNotHealthyException(string responseContent) : base(string.Format(ElasticSearchNotHealthy, responseContent))
    {
        
    }
}

public class ElasticSearchNotAvailableException : Exception
{
    private static readonly string ElasticSearchNotAvailable = ElasticClientResources.ElasticSearchNotAvailable;

    public ElasticSearchNotAvailableException(string address, int port, int seconds) : base(
        string.Format(ElasticSearchNotAvailable, address, port, seconds))
    {
    }
    public ElasticSearchNotAvailableException(string address, int seconds) : base(
        string.Format(ElasticSearchNotAvailable, address, null, seconds))
    {
    }
}