using System.Reflection;
using Newtonsoft.Json;
using PlatformEntities;
using Processors.Api;

namespace ProcessorsContainer;

public static class ProcessorCaller
{
    public static object? Process(IProcessor processor, string? jsonData, CancellationToken token)
    {
        var processorType = processor.GetType();
        var (tIn, tOut) = GetInputOutputTypes(processorType);
        var processorOutput = typeof(ProcessorResult<>);
        tOut = processorOutput.MakeGenericType(tOut);
        var input = jsonData == null ? null : JsonConvert.DeserializeObject(jsonData, tIn);
        var method = processorType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .First(mi => mi.ReturnType == tOut && mi.GetParameters().Any(p => p.ParameterType == tIn) &&
                         mi.Name == "Process");
        var result = method.Invoke(processor, new[] { input, token });
        return result;
    }
    private static (Type tIn, Type tOut) GetInputOutputTypes(Type containerType)
    {
        var iProcessorInterface = containerType.FindInterfaces(InterfaceFilter, typeof(IProcessor<,>).Name).First();
        var genericArguments = iProcessorInterface.GetGenericArguments();
        var tIn = genericArguments.First();
        var tOut = genericArguments.Last();
        return (tIn, tOut);
    }
    private static bool InterfaceFilter(Type typeObj, object? criteriaObj)
    {
        var typeName = typeObj.ToString();
        var criteriaOrEmpty = criteriaObj?.ToString() ?? string.Empty;
        return typeName.Contains(criteriaOrEmpty);
    }
}