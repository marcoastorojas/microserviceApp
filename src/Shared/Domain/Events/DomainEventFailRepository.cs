namespace Shared.Domain.Events;

public interface DomainEventFailRepository
{
    Task Save(DomainEvent domainEvent);
    Task<List<DomainEvent>> GetAll();
}