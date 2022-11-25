using System.Net.Http.Headers;
using System.Text;
using Processors.Api;

namespace ElasticClient.Entities;

[ProcessElement(nameof(ElasticRequestMessage), ProcessingAttributeBehaviourType.Input)]
public class ElasticRequestMessage
{
    public Dictionary<string, string> Headers { get; set; }
    public HttpMethod Method { get; set; }
    public Uri Address { get; set; }
    public string? Data { get; set; }

    public ElasticRequestMessage()
    {
        //only for deserialization
    }

    public ElasticRequestMessage(HttpMethod method, string uri, string? data = null, Dictionary<string, IEnumerable<string>>? headers = null,
        Version? version = null,
        HttpVersionPolicy versionPolicy = default)
    {
        Headers = new Dictionary<string, string>();
        Data = data;
        Address = new Uri(uri);
        Method = method;

        if (headers == null) return;
        foreach (var header in headers)
        {
            if (header.Value != null && header.Value.Count() > 1)
            {
                Headers.Add(header.Key, string.Join("; ", header.Value));
            }
            else
            {
                Headers.Add(header.Key, header.Value.FirstOrDefault());
            }
        }
    }

    public ElasticRequestMessage(HttpMethod method, string uri, string? data = null, HttpRequestHeaders? headers = null, Version version = null,
        HttpVersionPolicy versionPolicy = default)
    {
        Headers = new Dictionary<string, string>();
        Data = data;
        Address = new Uri(uri);
        Method = method;

        if (headers == null) return;
        foreach (KeyValuePair<string, IEnumerable<string>> header in headers)
        {
            if (header.Value != null && header.Value.Count() > 1)
            {
                Headers.Add(header.Key, string.Join("; ", header.Value));
            }
            else
            {
                Headers.Add(header.Key, header.Value.FirstOrDefault());
            }
        }
    }

    public ElasticRequestMessage(HttpMethod method, string uri, string? data = null, Dictionary<string, string>? headers = null, Version? version = null,
        HttpVersionPolicy versionPolicy = default)
    {
        Headers = new Dictionary<string, string>();
        Data = data;
        Address = new Uri(uri);
        Method = method;

        if (headers == null) return;
        foreach (var header in headers)
        {
            Headers.Add(header.Key, header.Value);
        }
    }


    public ElasticRequestMessage(HttpMethod method, string address, string? data = null)
    {
        Data = data;
        Address = new Uri(address);
        Method = method;
    }

    public HttpRequestMessage ToHttpMessage()
    {
        var message = new HttpRequestMessage()
        {
            Content = new StringContent(Data, Encoding.UTF8, "application/json"),
            Method = Method,
            RequestUri = Address,
        };
        if (Headers != null)
        {
            foreach (var header in Headers)
            {
                message.Headers.Add(header.Key, header.Value);
            }
        }

        return message;
    }
}