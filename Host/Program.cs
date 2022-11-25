namespace SuperAgent;

class Program
{
    public static async Task Main(string[] args)
    {
        var agentYaml = @"D:\Work\CBAP_1\Processors\Host\config\agent.yaml";
        var cfg = ConfigParser.Parse(agentYaml);
        var agent = new Agent(cfg);
        agent.Start();
        Console.ReadKey();
    }
}