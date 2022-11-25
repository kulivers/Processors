using ElasticClient.Contracts;
using ElasticClient.Entities.Credentials;
using Localization.Libs;

namespace ElasticClient.Config;

public class Authentication 
{
    private const string Basic = "BASIC";
    private const string ApiKey = "APIKEY";
    private const string Barrier = "BARRIER";
    private const string OAuth = "OAUTH";   
    private static readonly string UnknownAuthenticationType = ElasticClientResources.UnknownAuthenticationType;

    public Authentication(string type, string username, string password)
    {
        Type = type.ToUpper();
        Username = username;
        Password = password;
    }
    public Authentication(string type, string token)
    {
        Type = type.ToUpper();
        Token = token;
    }

    
    public Authentication()
    {
        //for deserializing only   
    }
    
    public string Type { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? Token { get; set; }
    public AuthenticationCredentials GetAuthCredentials()
    {
        switch (Type.ToUpper())
        {
            case Basic:
            {
                return Token == null
                    ? new BasicCredentials(Username!, Password!)
                    : new BasicCredentials(Token);
            }
            case ApiKey:
            {
                return Token == null
                    ? new ApiKeyCredentials(Username!, Password!)
                    : new ApiKeyCredentials(Token);
            }
            case Barrier:
            case OAuth:
            {
                return new BarrierCredentials(Token!);
            }
            default:
            {
                throw new NotSupportedException(UnknownAuthenticationType);
            }
        }
    }
    
}