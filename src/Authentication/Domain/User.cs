using System.Reflection.Metadata.Ecma335;
using Shared.Domain.Aggregates;

namespace Authentication.Domain;

public class User(string id, string name, string email) : AggregateRoot
{
    public string Id = id;
    public string Name = name;
    public string Email = email;

    public static User Create(string id, string name, string email){
        var user = new User(id,name,email);
        user.Record(new UserCreated(id, name, email));
        return user;
    }
    

}