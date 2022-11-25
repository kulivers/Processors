using ElasticClient.Contracts;
using Localization.Libs;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ElasticClient.Config;

public class EsManagerConfig
{
    private static readonly string WrongTypeOfFileNeedToBeYaml = UtilResources.WrongTypeOfFileNeedToBeYaml;
    public string Host { get; set; }
    public Authentication Authentication { get; set; }

    public EsManagerConfig()
    {
        //for deserialization
    }

    public EsManagerConfig(string host, Authentication authentication)
    {
        Host = host;
        Authentication = authentication;
    }

    public AuthenticationCredentials GetAuthCredentials() => Authentication.GetAuthCredentials();

    public static EsManagerConfig FromYaml(string path)
    {
        if (!path.EndsWith(".yaml"))
        {
            throw new ArgumentException(WrongTypeOfFileNeedToBeYaml);
        }

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        var fileContent = File.ReadAllText(path);
        return deserializer.Deserialize<EsManagerConfig>(fileContent);
    }
}