using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Shared.Infrastrucure.Events.RabbitMq;


public class RabbitMqConnection
{
    private readonly ConnectionFactory ConnectionFactory;
    private IModel? _channel;
    private IConnection? _connection;
    public RabbitMqConnection(IOptions<RabbitMqConfigParams> rabbitMqParams)
    {
        var configParams = rabbitMqParams.Value;

        ConnectionFactory = new ConnectionFactory
        {
            HostName = configParams.HostName,
            UserName = configParams.Username,
            Password = configParams.Password,
            Port = configParams.Port
        };
        
    }

    public IConnection Connection()
    {
        if (_connection == null) _connection = ConnectionFactory.CreateConnection();
        return _connection;
    }


    public IModel Channel()
    {
        if (_channel == null) _channel = Connection().CreateModel();
        return _channel;
    }
}


public class RabbitMqConfigParams
{
    public string HostName {get; set;} = "";
    public string Username {get; set;} = "";
    public string Password {get; set;} = "";
    public int Port {get; set;}
}