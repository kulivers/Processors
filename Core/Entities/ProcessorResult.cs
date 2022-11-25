namespace PlatformEntities;

public interface IProcessorResult
{
    bool Success { get; set; }
    object? GetData();
    Exception? Exception { get; set; }
}
public interface IProcessorResult<T> : IProcessorResult
{
    public T? Data { get; set; }
}

public class ProcessorResult<T> : IProcessorResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public object? GetData()
    {
        return Data;
    }

    public Exception? Exception { get; set; }

    public ProcessorResult(T? data)
    {
        Success = true;
        Data = data;
        Exception = null;
    }

    public ProcessorResult(Exception exception)
    {
        Success = false;
        Exception = exception;
    }
}