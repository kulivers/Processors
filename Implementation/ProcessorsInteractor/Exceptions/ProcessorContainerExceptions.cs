using Localization.Processors;

namespace ProcessorsContainer.Exceptions;

public class UnknownProcessorException : Exception
{
    private static readonly string UnknownProcessor = ProcessorResources.UnknownProcessor;

    public UnknownProcessorException(string name) : base(string.Format(UnknownProcessor, name))
    {
        
    }
}
public class CantLoadServiceException : Exception
{
    private static readonly string CantLoadService = ProcessorResources.CantLoadService;
    

    public CantLoadServiceException(string service) : base(string.Format(CantLoadService, service))
    {
        
    }
}