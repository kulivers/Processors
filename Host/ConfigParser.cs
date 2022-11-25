using IOServices.Api;
using IOServices.Api.Input;
using IOServices.Api.Input.Configs;
using IOServices.Api.Output;
using IOServices.Api.Output.Configs;
using Localization.Libs;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SuperAgent;

public static class ConfigParser
{
    private static readonly string WrongTypeOfFileNeedToBeYaml = UtilResources.WrongTypeOfFileNeedToBeYaml;

    private static IDeserializer Deserializer => new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();

    public static AgentConfig Parse(string path)
    {
        var agentConfig = ParseAgentConfig(path);
        foreach (var config in agentConfig.ConnectorsConfigs)
        {
            var inputCfgContent = File.ReadAllText(config.InputServiceConfigPath);
            config.InputConfig = ParseInputConfig(inputCfgContent, config.InputType);

            var outputCfgContent = config.OutputServiceConfigPath == null ? null : File.ReadAllText(config.OutputServiceConfigPath);

            if (outputCfgContent != null && config.OutputType != null)
            {
                config.OutputConfig = ParseOutputConfig(outputCfgContent, (OutputServiceType)config.OutputType);
            }
        }

        return agentConfig;
    }

    private static AgentConfig ParseAgentConfig(string path)
    {
        if (!path.EndsWith(".yaml"))
        {
            throw new ArgumentException(WrongTypeOfFileNeedToBeYaml);
        }

        var fileContent = File.ReadAllText(path);
        var agentConfig = Deserializer.Deserialize<AgentConfig>(fileContent);
        return agentConfig;
    }

    private static IInputServiceConfig ParseInputConfig(string content, InputServiceType inputServiceType)
    {
        return inputServiceType switch
        {
            InputServiceType.Kafka => Deserializer.Deserialize<KafkaInputConfig>(content),
            InputServiceType.Memory => Deserializer.Deserialize<MemoryInputConfig>(content),
            _ => throw new ArgumentException("Cant find next values in config: \"outputType\"")
        };
    }

    private static IOutputServiceConfig ParseOutputConfig(string content, OutputServiceType outputServiceType)
    {
        return outputServiceType switch
        {
            OutputServiceType.Kafka => Deserializer.Deserialize<KafkaOutputConfig>(content),
            OutputServiceType.Memory => Deserializer.Deserialize<MemoryOutputConfig>(content),
            OutputServiceType.Combined => CreateCombinedConfig(content),
            _ => throw new ArgumentOutOfRangeException(nameof(outputServiceType), outputServiceType, null)
        };
    }

    private static CombinedOutputConfig CreateCombinedConfig(string content)
    {
        var config = Deserializer.Deserialize<CombinedOutputConfig>(content);
        var outputServiceConfigs = config.OutputServicesPathTypes.Select(pathType =>
        {
            var serviceConfigContent = File.ReadAllText(pathType.Path);
            return ParseOutputConfig(serviceConfigContent, pathType.Type);
        }).ToArray();
        config.OutputServiceConfigs = outputServiceConfigs;
        return config;
    }
}