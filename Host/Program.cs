namespace SuperAgent;

class Program
{
    public static async Task Main(string[] args)
    {
        var agentYaml = @"/home/asto/egor/Processors/Host/config/agent.yaml";
        var cfg = ConfigParser.Parse(agentYaml);
        var agent = new Agent(cfg);
        agent.Start();
        Console.ReadKey();
    }
}