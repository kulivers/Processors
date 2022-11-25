namespace IOServices.Api.Output.Configs;

public class CombinedOutputConfig : IOutputServiceConfig
{
    public ServicePathType[] OutputServicesPathTypes;
    public IOutputServiceConfig[] OutputServiceConfigs;
    public ToSendFilterConfig? ToSendFilter;
    public CombinedOutputMode Mode;
}
public struct ServicePathType
{
    public string Path;
    public OutputServiceType Type;
}
public enum CombinedOutputMode
{
    OneByOneAll,
    OneByOneIfNotSuccess
}
