
namespace Shared.Domain.Events;

public interface DomainEventSubscriber<TDomain> : DomainEventSubscriberBase where TDomain: DomainEvent
{
    async Task DomainEventSubscriberBase.Handle(DomainEvent @event){
        if (@event is TDomain msg)
        {
            await Handle(msg);
        }
    }
    Task Handle(TDomain domainEvent);
}