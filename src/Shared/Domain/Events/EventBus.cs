namespace Shared.Domain.Events;


public interface EventBus
{
    Task Publish(List<DomainEvent> domainEvents);
}