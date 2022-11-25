using IOServices.Api;
using IOServices.Api.Input;
using PlatformEntities;

namespace IOServices.Input.Memory;

public class MemoryInput : IInputService<string>
{
    private readonly List<string> _dirs;
    public event EventHandler<IInputServiceMessage>? OnReceive;
    public event EventHandler<IInputServiceMessage<string>>? OnReceiveT;
    public MemoryInput(IEnumerable<string> dirToFind)
    {
        _dirs = dirToFind.ToList();
    }

    public async Task StartReceive(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            foreach (var dir in _dirs)
            {
                var files = Directory.GetFiles(dir).Where(f => f.EndsWith(".json"));
                foreach (var path in files)
                {
                    var content = await File.ReadAllTextAsync(path, token);
                    var message = new InputServiceMessage<string>(content, path, true);
                    OnReceive?.Invoke(this, message);
                    OnReceiveT?.Invoke(this, message);
                }
            }
        }
    }

    public void OnResponse(IInputServiceMessage input, IProcessorResult processorResult)
    {
        OnResponse((IInputServiceMessage<string>)input, processorResult);
    }

    public void OnResponse(IInputServiceMessage<string> request, IProcessorResult response)
    {
        if (response.Success)
        {
            File.Delete(request.Id);
        }
    }


    public void CheckHealth(double secondsToResponse)
    {
        foreach (var dir in _dirs.Where(dir => !Directory.Exists(dir)))
        {
            Directory.CreateDirectory(dir);
        }
    }
}