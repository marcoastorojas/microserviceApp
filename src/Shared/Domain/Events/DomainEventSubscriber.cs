
using Microsoft.VisualBasic;

namespace Shared.Domain.Events;

public abstract class DomainEventSubscriber<TDomain> : DomainEventSubscriberBase where TDomain: DomainEvent
{
    async Task DomainEventSubscriberBase.Handle(DomainEvent @event){
        if (@event is TDomain msg)
        {
            await Handle(msg);
        }
    }
    public abstract Task Handle(TDomain domainEvent);
}