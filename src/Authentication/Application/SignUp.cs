using Authentication.Domain;
using Shared.Domain.Events;

namespace Authentication.Applicacion;
public class SignUp(EventBus eventBus)
{
    EventBus eventBus = eventBus;

    public async Task Crear(){
        var userCreated = User.Create(Guid.NewGuid().ToString(),"marco","marcoasto@gmail.com");
        await eventBus.Publish(userCreated.Pull());
    }
}