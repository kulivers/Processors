using IOServices.Api.Input;

namespace Connector.Api;

public interface IProxyTypeMapper
{
    string Map(IInputServiceMessage input);
}

