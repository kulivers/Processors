using System.Reflection;
using Processors.Api;
using ProcessorsContainer.Exceptions;

namespace ProcessorsContainer;

public static class ProcessorFactory
{
    public static IProcessor CreateProcessor(string dll, string config, string name)
    {
        var assembly = Assembly.LoadFrom(dll);
        var assTypes = assembly.GetTypes();
        var factories = FindProcessorFactory(assTypes);
        foreach (var factoryType in factories)
        {
            var factoryInstance = Activator.CreateInstance(factoryType);
            var factoryMethodName = typeof(IProcessorFactory<,>).GetMethods().First().Name;
            var factoryMethod = factoryType.GetMethod(factoryMethodName);
            var serviceInstance = factoryMethod?.Invoke(factoryInstance, new object[] { dll, config, name });
            if (serviceInstance is IProcessor processor)
            {
                return processor;
            }
        }

        throw new CantLoadServiceException(
            $"Cant find processor factory type in {dll} assembly. Try next steps:" +
            "0. Rebuild your project" +
            "1. You need to implement IProcessorFactory interface in this assembly " +
            "2. Mark with ProcessingAttributeBehaviourType.Factory attribute on factory class");
    }

    private static IEnumerable<Type> FindProcessorFactory(IEnumerable<Type> types)
    {
        var typesWithAttribute = types.Where(it =>
            it.GetCustomAttributes().Any(attribute => attribute.GetType() == typeof(ProcessElementAttribute)));
        foreach (var type in typesWithAttribute)
        {
            foreach (var attribute in type.GetCustomAttributes())
            {
                if (attribute is ProcessElementAttribute { Type: ProcessingAttributeBehaviourType.Factory })
                {
                    yield return type;
                }
            }
        }
    }
}