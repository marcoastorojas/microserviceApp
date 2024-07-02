
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Shared.Domain.Events;
using Shared.Infrastrucure.Events.RabbitMq;

namespace Shared.Infrastrucure.DependencyInjection;
public static class  DomainEventsSubscriptionsService
{
    public static IServiceCollection AddDomainEventsSubscriptions(this IServiceCollection services, Assembly assembly)
    {
        var domainEventsSubscriptions = new DomainEventSubscriptions();

        var classTypes = assembly.ExportedTypes.Select(t => t.GetTypeInfo()).Where(t => t.IsClass && !t.IsAbstract);

        foreach (var type in classTypes)
        {
            var interfaces = type.ImplementedInterfaces.Select(i => i.GetTypeInfo());

            foreach (var handlerInterfaceType in interfaces.Where(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(DomainEventSubscriber<>)))
            {
                services.AddScoped(handlerInterfaceType.AsType(), type.AsType());
                FormatSubscribers(assembly, handlerInterfaceType, domainEventsSubscriptions);
            }
        }

        services.AddScoped(s => domainEventsSubscriptions);
        return services;
    }

    private static void FormatSubscribers(Assembly assembly, TypeInfo handlerInterfaceType,DomainEventSubscriptions domainEventSubscriptions)
    {
        var handlerClassTypes = assembly.GetLoadableTypes()
            .Where(handlerInterfaceType.IsAssignableFrom);

        var domainEventSubscription = handlerInterfaceType.GenericTypeArguments.FirstOrDefault()!;

        if (domainEventSubscription == null) return;

        foreach (var domainEventSubscriber in handlerClassTypes)
            domainEventSubscriptions.Register(
                new TDomainEventSubscriber(domainEventSubscriber), 
                new TDomainEventSubscription(domainEventSubscription)
            );
    }

    private static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types.Where(t => t != null);
        }
    }
}