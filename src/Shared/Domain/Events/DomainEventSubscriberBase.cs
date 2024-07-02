
namespace Shared.Domain.Events;
public interface DomainEventSubscriberBase
{
    Task Handle(DomainEvent @event);
}
