
using System.Globalization;

namespace Shared.Infrastrucure.Events.RabbitMq;


public class DomainEventSubscriptions
{
    public Dictionary<TDomainEventSubscriber,TDomainEventSubscription> Subscriptions {get;} = [];
    public void Register(TDomainEventSubscriber subscriber, TDomainEventSubscription subscription){
        
        Subscriptions.TryAdd(subscriber, subscription);
    }

}

public class TDomainEventSubscription(Type evento)
{
    public readonly Type evento = evento;

    public string EventName()
    {
        return string.Concat( evento.Name.Select((x, i) =>
                i > 0 && char.IsUpper(x) ? "_" + x : x.ToString(CultureInfo.InvariantCulture)))
            .ToLowerInvariant();
    }
}

public class TDomainEventSubscriber(Type subscriber)
{
    readonly public Type subscriber  = subscriber;

    public string QueueName()
    {
        return string.Concat( subscriber.Name.Select((x, i) =>
                i > 0 && char.IsUpper(x) ? "_" + x : x.ToString(CultureInfo.InvariantCulture)))
            .ToLowerInvariant();
    }
}