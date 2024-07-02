namespace Shared.Domain.Events;

public abstract class DomainEvent
{

    public string AggregateId;
    public string EventId;
    public DateTime OccurredOn;

    public DomainEvent(){

    }
    
    public DomainEvent(string aggregateId){
        EventId = Guid.NewGuid().ToString();
        OccurredOn = DateTime.Now;
        AggregateId = aggregateId;
    }

    public abstract string EventName();
    public abstract Dictionary<string, string> ToPrimitives();
    public abstract DomainEvent FromPrimitives(string aggregateId, Dictionary<string, string> body, string eventId,
            string occurredOn);
}