namespace IOServices.Api.Output;

public class OutputResponseModel
{ 
    public bool IsSuccess { get; set; }
    public bool WasSent { get; set; }
    public IResource WasSentTo { get; set; }
    public object? Data { get; set; }
    public Exception? Exception { get; set; }
    public IEnumerable<OutputResponseModel>? OutputModels { get; set; }
    public OutputResponseModel()
    {
    }
    public OutputResponseModel(IEnumerable<OutputResponseModel> outputModels)
    {
        var enumerable = outputModels as OutputResponseModel[] ?? outputModels.ToArray();
        Data = null;
        Exception = null;
        if (enumerable.All(model => model.IsSuccess))
        {
            IsSuccess = true;
        }
        OutputModels = enumerable;
        WasSent = true;
    }
    public OutputResponseModel(IResource resource, object? data, bool wasSent)
    {
        WasSentTo = resource;
        IsSuccess = true;
        Exception = null;
        Data = data;
        WasSent = wasSent;
    }

    public OutputResponseModel(IResource resource, Exception exception)
    {
        WasSentTo = resource;
        WasSent = true;
        IsSuccess = false;
        Exception = exception;
        Data = null;
    }
}