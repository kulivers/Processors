using PlatformEntities;

namespace IOServices.Api.Output;

public interface IOutputServiceFilter
{
    bool ShouldSend(IProcessorResult? toSend);
}