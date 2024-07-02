using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using Shared.Domain.Events;

namespace Shared.Infrastrucure.Events.RabbitMq;


public class RabbitMqConsumer(DomainEventSubscriptions domainEventSubscriptions, IServiceProvider serviceProvider, RabbitMqConnection connection)
{
    // DomainEventSubscriptions ->  <DomainEventSubscriber, DomainEventSubscription> 
    // que se obtienen de la lectura de la solucion a travez de un comando al iniciar;
    private readonly DomainEventSubscriptions domainEventSubscriptions = domainEventSubscriptions;
    
    // RabbitMqConnection ->  conexion del cliente Rabbit que da acceso a un Canal ya abierto;
    private RabbitMqConnection rabbitMqConnection = connection;

    // IServiceProvider -> que nos da la relacion de clases declaradas en la solucion;
    private IServiceProvider serviceProvider = serviceProvider;

    // DomainEventSubscribers servira para tener un registro de las instancias generadas 
    // y no volverlas  a crear 
    private readonly Dictionary<string, object> domainEventSubscribers = [];


    public async Task Consume(){
        //Se consume las subscripciones que se generaron con el mapeo de los domainSubcscriber y sus eventos asociados
        foreach (var domainEventSubscription in domainEventSubscriptions.Subscriptions)
        {
            var subscriber = domainEventSubscription.Key; 
            var subscription = domainEventSubscription.Value;

            
            // Se buscara si existe el subscriber en domainEventSubscribers si no se instanciará y guardará
            var instanciaSubscriber = domainEventSubscribers.ContainsKey(subscriber.QueueName()) ? 
                domainEventSubscribers[subscriber.QueueName()] :
                ObtenerInstanciaDomainEventSubscriber(subscriber);
            
            // Ahora se debe Subscribir el evento a la instancia usando rabbit
            var _channel = rabbitMqConnection.Channel();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.Span);
                
                // en este punto se debe ejecutar el metodo handle de la instancia
                var domainEvent = Deserialize(message,subscription.evento);

                await ((DomainEventSubscriberBase) subscriber).Handle(domainEvent);
            };

        }
    }

    private object ObtenerInstanciaDomainEventSubscriber(TDomainEventSubscriber domainEventSubscriber){
        // Obtenemos la instancia del subscriber
        var subscriber = domainEventSubscriber.subscriber;
        var scope = serviceProvider.CreateScope();
        var instancia = scope.ServiceProvider.GetRequiredService(subscriber);
        // Se agrega al diccionario para no volverlo a instanciar
        domainEventSubscribers[domainEventSubscriber.QueueName()] = instancia;
        return instancia;
    }


    public DomainEvent Deserialize(string body, Type typeDomainEvent)
        {
            var eventData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(body);

            var data = eventData!["data"];
            var attributes = JsonConvert.DeserializeObject<Dictionary<string, string>>(data["attributes"].ToString()!)!;

            var simpleInstance = (DomainEvent) Activator.CreateInstance(typeDomainEvent)!;
            var domainEvent = simpleInstance.FromPrimitives(
                attributes["id"] ?? "",
                attributes,
                data["id"].ToString() ?? "",
                data["ocurred on"].ToString() ?? ""
            );
            return domainEvent;
        }

}