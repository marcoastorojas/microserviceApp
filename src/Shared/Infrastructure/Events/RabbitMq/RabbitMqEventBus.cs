namespace Shared.Infrastrucure.Events.RabbitMq;

using System.Text;
using System.Text.Json;
using Shared.Domain.Events;


public class RabbitMqEventBus : EventBus
{
    private string _exchangeName;
    private readonly RabbitMqConnection  Connection;

    public RabbitMqEventBus(RabbitMqConnection connection, string exchangeName = "domain_events")
    {
        Connection = connection;
        _exchangeName = exchangeName;
    }

    public async Task Publish(List<DomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await Publish(domainEvent);
        }
    }

    public async Task Publish(DomainEvent domainEvent){
        
        // SERIALIZAR PARA PODER CONVERTIRLO A BYTES
        var eventSerialized = Serialize(domainEvent);
        // TRANSFORMAR A BYTES PARA RABBITMQ
        var body = Encoding.UTF8.GetBytes(eventSerialized);
        
        var channel = Connection.Channel();
        
        channel.BasicPublish(
            _exchangeName,
            domainEvent.EventName(),
            true,
            channel.CreateBasicProperties(), // TODO agregar el numero de reenvio;
            body
        );
    }

    
    public static string Serialize(DomainEvent domainEvent)
    {
        try
        {
            if (domainEvent == null) return "";

            var attributes = domainEvent.ToPrimitives();

            attributes.Add("id", domainEvent.AggregateId);

            return JsonSerializer.Serialize(new Dictionary<string, Dictionary<string, object>>
            {
                {
                    "data", new Dictionary<string, object>
                    {
                        {"id", domainEvent.EventId},
                        {"type", domainEvent.EventName()},
                        {"occurred_on", domainEvent.OccurredOn},
                        {"attributes", attributes}
                    }
                },
                {"meta", new Dictionary<string, object>()}
            });
        }
        catch (System.Exception ex)
        {
            
            throw;
        }
        
    }


}
