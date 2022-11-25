namespace IOServices.Api.Input;

public interface IInputServiceMessage<out T> : IInputServiceMessage
{
    public T Data { get; }
}

public interface IInputServiceMessage
{
    public string Id { get; set; }
    public object GetData();
    public bool IsJson { get; set; }
}

public class InputServiceMessage<T> : IInputServiceMessage<T>
{
    public string Id { get; set; }
    public T Data { get; }
    public object GetData() => Data;

    public bool IsJson { get; set; }

    public InputServiceMessage(T data, string id, bool isJson = false)
    {
        Data = data;
        Id = id;
        IsJson = isJson;
    }
}