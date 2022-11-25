using System.Net.Http.Headers;
using ElasticClient.Config;
using ElasticClient.Contracts;

namespace ElasticClient;

public class EsClient : HttpClient
{
    private const string AuthorizationHeaderKey = "Authorization";
    private const string ContentTypeHeaderValue = "application/json";
    public EsClient(EsManagerConfig esManagerConfig)
    {
        BaseAddress = new Uri(esManagerConfig.Host);
        AuthenticationCredentials authCredentials = esManagerConfig.GetAuthCredentials();
        if (authCredentials != null)
        {
            DefaultRequestHeaders.Add(AuthorizationHeaderKey, authCredentials.ToHeaderValue());
        }

        var acceptHeader = new MediaTypeWithQualityHeaderValue(ContentTypeHeaderValue);
        DefaultRequestHeaders.Accept.Add(acceptHeader); //ACCEPT header
    }

    public void SetCredentials(AuthenticationCredentials credentials)
    {
        DefaultRequestHeaders.Add(AuthorizationHeaderKey, credentials.ToHeaderValue());
    }
}