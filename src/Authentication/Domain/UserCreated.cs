using Microsoft.VisualBasic;
using Shared.Domain.Events;

namespace Authentication.Domain;

public class UserCreated : DomainEvent
{
    public override string EventName()
    {
        return "user.created";
    }
    string Name;
    string Email;

    public UserCreated(string aggregateId,string name, string email ):base(aggregateId){
        Name = name;
        Email = email;
    }   
    public UserCreated(){}

    public UserCreated(string aggregateId, string eventId, DateTime createdOn, Dictionary<string, string> body): 
        base(aggregateId, eventId, createdOn)
    {
        Name = body["name"];
        Email = body["email"];
    }
    public override DomainEvent FromPrimitives(string aggregateId, Dictionary<string, string> body, string eventId, string occurredOn)
    {
        return new UserCreated(aggregateId, body["name"], body["email"])
        {
            AggregateId = aggregateId,
            EventId = eventId,
            OccurredOn = DateTime.Parse(occurredOn)
            
        };
    }

    public override Dictionary<string, string> ToPrimitives()
    {
        Dictionary<string, string> body = [];
        body.Add("email",Email);
        body.Add("name",Name);
        return body;
    }
}