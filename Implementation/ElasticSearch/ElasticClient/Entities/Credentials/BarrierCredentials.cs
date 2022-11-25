using ElasticClient.Contracts;

namespace ElasticClient.Entities.Credentials;

public class BarrierCredentials : AuthenticationCredentials
{
    public override string Type => "Barrier";
    public override string Token { get; set; }

    public BarrierCredentials(string token)
    {
        Token = token;
    }
}