namespace Shared.Infrastructure.Events.RabbitMq;

using System.Text;
using System.Text.Json;
using Microsoft.VisualBasic;
using Shared.Domain.Events;
using Shared.Infrastructure.Events.RabbitMq;

public class RabbitMqEventBus : EventBus
{
    private string _exchangeName;
    private readonly RabbitMqConnection  Connection;
    private readonly DomainEventFailRepository domainEventFailRepository;

    public RabbitMqEventBus(RabbitMqConnection connection, DomainEventFailRepository domainEventFailRepository, string exchangeName = "domain_events")
    {
        Connection = connection;
        _exchangeName = exchangeName;
        this.domainEventFailRepository = domainEventFailRepository;
    }

    public async Task Publish(List<DomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await Publish(domainEvent);
        }
    }

    public async Task Publish(DomainEvent domainEvent){
        try
        {
            // SERIALIZAR PARA PODER CONVERTIRLO A BYTES
            var eventSerialized = Serialize(domainEvent);
            // TRANSFORMAR A BYTES PARA RABBITMQ
            var body = Encoding.UTF8.GetBytes(eventSerialized);
            
            var channel = Connection.Channel();

            var properties = channel.CreateBasicProperties();
            properties.Headers = new Dictionary<string, object>
            {
                {"redelivery_count", 0}
            };
            
            channel.BasicPublish(
                _exchangeName,
                domainEvent.EventName(),
                true,
                properties, // TODO agregar el numero de reenvio;
                body
            );
            
        }
        catch (Exception ex)
        {
            revisarError(ex.Message);
            await domainEventFailRepository.Save(domainEvent);
        }
    }

    void revisarError(string error){
        Console.WriteLine(error);
    }
    
    public static string Serialize(DomainEvent domainEvent)
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


}
