
namespace Authentication.Applicacion;
using Authentication.Domain;
using Shared.Domain.Events;

public class MakeSomethingUserCreated : DomainEventSubscriber<UserCreated>
{
    public Task Handle(UserCreated domainEvent)
    {
        throw new NotImplementedException();
    }
}