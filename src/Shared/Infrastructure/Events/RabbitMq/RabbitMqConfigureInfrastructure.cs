namespace Shared.Infrastructure.Events.RabbitMq;

public class RabbitMqConfigureInfrastructure(DomainEventSubscriptions domainEventSubscriptions, RabbitMqConnection rabbitMqConnection)
{   
    private readonly DomainEventSubscriptions domainEventSubscriptions = domainEventSubscriptions;
    private readonly RabbitMqConnection rabbitMqConnection = rabbitMqConnection;

    public void Configure(){
        
        var _channel = rabbitMqConnection.Channel();
        foreach (var domainEventSubscription in domainEventSubscriptions.Subscriptions)
        {
            var subscriber = domainEventSubscription.Key; 
            var subscription = domainEventSubscription.Value;

            _channel.QueueDeclare(subscriber.QueueName(), true, false, true, null);
            _channel.QueueDeclare(subscriber.QueueName("retry"), true, false, true, RetryQueueArguments("domain_events",subscriber.QueueEventName()));
            _channel.QueueDeclare(subscriber.QueueName("dead_letter"), true, false, true, null);
            
            
            _channel.QueueBind(
                subscriber.QueueName("retry"), 
                "retry_domain_events",
                subscriber.QueueEventName("retry"),
                new Dictionary<string, object>()
            );
            _channel.QueueBind(subscriber.QueueName("dead_letter"), "dead_letter_domain_events", subscriber.QueueEventName("dead_letter"),new Dictionary<string, object>());
            
            _channel.QueueBind(subscriber.QueueName(), "domain_events", subscription.EventName(),new Dictionary<string, object>());
            _channel.QueueBind(subscriber.QueueName(), "domain_events", subscriber.QueueEventName(),new Dictionary<string, object>());
        }
    }   


    private IDictionary<string, object> RetryQueueArguments(string domainEventExchange, string domainEventQueue)
    {
        return new Dictionary<string, object>
        {
            {"x-dead-letter-exchange", domainEventExchange},
            {"x-dead-letter-routing-key", domainEventQueue},
            {"x-message-ttl", 5000}
        };
    } 
}