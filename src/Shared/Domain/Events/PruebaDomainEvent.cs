
namespace Shared.Domain.Events;

public class PruebaDomainEvent : DomainEvent
{
    public override string EventName()
    {
        throw new NotImplementedException();
    }

    public override DomainEvent FromPrimitives(string aggregateId, Dictionary<string, string> body, string eventId, string occurredOn)
    {
        throw new NotImplementedException();
    }

    public override Dictionary<string, string> ToPrimitives()
    {
        throw new NotImplementedException();
    }
}