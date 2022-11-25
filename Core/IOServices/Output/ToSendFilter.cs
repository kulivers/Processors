using IOServices.Api.Output;
using IOServices.Api.Output.Configs;
using PlatformEntities;

namespace IOServices.Output;

public class ToSendFilter : IOutputServiceFilter
{
    private readonly bool _sendErrors;
    private readonly bool _sendNullValues;
    private readonly bool _sendSuccesses;

    public ToSendFilter(ToSendFilterConfig filterConfig)
    {
        _sendErrors = filterConfig.Errors;
        _sendNullValues = filterConfig.NullValues;
        _sendSuccesses = filterConfig.Successes;
    }

    public bool ShouldSend(IProcessorResult? toSend)
    {
        if (toSend == null)
        {
            if (_sendNullValues)
            {
                return true;
            }

            return false;
        }


        if (!_sendErrors && toSend.Exception != null)
        {
            return false;
        }

        if (!_sendSuccesses && toSend.Success)
        {
            return false;
        }

        return true;
    }
}