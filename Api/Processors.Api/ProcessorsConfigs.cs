namespace Processors.Api;

public class ProcessorConfig
{
    public string Dll { get; set; }
    public string Config { get; set; }
    public string Name { get; set; }

    public ProcessorConfig()
    {
        
    }

    public ProcessorConfig(string dll, string config, string name)
    {
        Dll = dll;
        Config = config;
        Name = name;
    }

    public override bool Equals(object? otherObj)
    {
        if (otherObj is not ProcessorConfig other)
            return false;
        return Dll == other.Dll && Config == other.Config && Name == other.Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Dll, Config, Name);
    }
}