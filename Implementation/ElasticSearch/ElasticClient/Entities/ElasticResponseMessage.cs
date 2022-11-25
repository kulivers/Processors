using System.Net;
using Processors.Api;

namespace ElasticClient.Entities;

[ProcessElement(nameof(ElasticResponseMessage), ProcessingAttributeBehaviourType.Output)]
public class ElasticResponseMessage
{
    public HttpStatusCode? StatusCode { get; set; }
    public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; set; }
    public string Data { get; set; }

    public ElasticResponseMessage(HttpStatusCode? statusCode, string data, IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
    {
        StatusCode = statusCode;
        Data = data;
        Headers = headers;
    }
}