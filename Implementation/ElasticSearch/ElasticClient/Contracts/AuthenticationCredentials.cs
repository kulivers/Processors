namespace ElasticClient.Contracts;

public abstract class AuthenticationCredentials
{
    public abstract string Type { get;}
    public abstract string Token { get; set; }
    public virtual string ToHeaderValue() => $"{Type} {Token}";
}