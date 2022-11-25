namespace IOServices.Api.Output.Configs;

public class MemoryOutputConfig : IOutputServiceConfig
{
    public ToSendFilterConfig? ToSendFilter;
    public string DirToSaveDocs;
}