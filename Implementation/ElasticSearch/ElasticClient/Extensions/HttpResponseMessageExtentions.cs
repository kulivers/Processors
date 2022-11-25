using System.Net;
using ElasticClient.Entities;

namespace ElasticClient.Extensions;

public static  class HttpResponseMessageExtentions
{
    public static ElasticResponseMessage ToEsResponse(this HttpResponseMessage message)
    {
        var body = message.Content.ReadAsStringAsync().Result;
        var headers = (IEnumerable<KeyValuePair<string, IEnumerable<string>>>)message.Headers;
        return new ElasticResponseMessage(message.StatusCode, body, headers);
    }
   public static ElasticResponseMessage ToEsResponse(this HttpResponseMessage message, ElasticRequestMessage requestMessage)
    {
        var body = message.Content.ReadAsStringAsync().Result;
        var headers = (IEnumerable<KeyValuePair<string, IEnumerable<string>>>)message.Headers;
        return new ElasticResponseMessage((HttpStatusCode?)message.StatusCode, body, headers);
    }
    public static async Task<ElasticResponseMessage> ToEsResponseAsync(this HttpResponseMessage message)
    {
        var body = await message.Content.ReadAsStringAsync();
        var headers = (IEnumerable<KeyValuePair<string, IEnumerable<string>>>)message.Headers;
        return new ElasticResponseMessage(message.StatusCode, body, headers);
    }
    public static async Task<ElasticResponseMessage> ToEsResponseAsync(this HttpResponseMessage message, ElasticRequestMessage requestMessage)
    {
        var body = await message.Content.ReadAsStringAsync();
        var headers = (IEnumerable<KeyValuePair<string, IEnumerable<string>>>)message.Headers;
        return new ElasticResponseMessage((HttpStatusCode?)message.StatusCode, body, headers);
    }
}