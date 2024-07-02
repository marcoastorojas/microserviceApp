using Shared.Domain.Events;

namespace Shared.Domain.Aggregates;

public abstract class AggregateRoot
{
    List<DomainEvent> Events = [];

    public void Record(DomainEvent domainEvent){
        Events.Add(domainEvent);
    }


    public List<DomainEvent> Pull(){
        var events = Events;
        Events = [];
        return events;
    }
    
}