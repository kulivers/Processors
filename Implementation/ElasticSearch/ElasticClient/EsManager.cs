using ElasticClient.Config;
using ElasticClient.Contracts;
using ElasticClient.Entities;
using ElasticClient.Exceptions;
using ElasticClient.Extensions;

namespace ElasticClient;

public class EsManager
{
    private EsManagerConfig _esManagerConfig;
    private string ElasticHost { get; }
    private EsClient Client { get; }

    public EsManager(string elasticHost, EsClient client)
    {
        ElasticHost = elasticHost;
        Client = client;
    }

    public EsManager(EsManagerConfig esManagerConfig)
    {
        ElasticHost = esManagerConfig.Host;
        _esManagerConfig = esManagerConfig; 
        Client = new EsClient(esManagerConfig);
    }

    public void SetCredentialsToClient(AuthenticationCredentials credentials)
    {
        Client.SetCredentials(credentials);
    }

    public async Task CheckElasticAvailable(double secondsToResponse)
    {
        var requestIri = new Uri($"{ElasticHost}/_cat/health");
        var delay = TimeSpan.FromSeconds(secondsToResponse);
        var cts = new CancellationTokenSource(delay);
        try
        {
            var responseMessage = await Client.GetAsync(requestIri, cts.Token);
            if (!responseMessage.IsSuccessStatusCode)
            {
                var responseContent = await responseMessage.Content.ReadAsStringAsync(CancellationToken.None);
                throw new ElasticSearchNotHealthyException(responseContent);
            }
        }
        catch (HttpRequestException)
        {
            throw new ElasticSearchNotAvailableException(ElasticHost, Convert.ToInt32(secondsToResponse));
        }
        catch (TaskCanceledException)
        {
            throw new ElasticSearchNotAvailableException(ElasticHost, Convert.ToInt32(secondsToResponse));
        }
    }

    public ElasticResponseMessage SendRequest(ElasticRequestMessage elasticRequestMessage, CancellationToken token)
    {
        var httpMessage = elasticRequestMessage.ToHttpMessage();
        var response = Client.Send(httpMessage, token);
        var esResponse = response.ToEsResponse(elasticRequestMessage);
        return esResponse;
    }

    public async Task<ElasticResponseMessage> SendRequestAsync(ElasticRequestMessage elasticRequestMessage, CancellationToken token)
    {
        var httpMessage = elasticRequestMessage. ToHttpMessage();
        var response = await Client.SendAsync(httpMessage, token);
        var asEsResponse = await response.ToEsResponseAsync(elasticRequestMessage);
        return asEsResponse;
    }
}