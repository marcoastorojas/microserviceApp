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

    public IModel Channel(){
        if(Channel == null){
            _connection ??= ConnectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
        }
        return _channel!;
    }
}


public class RabbitMqConfigParams
{
    public string HostName {get; set;} = "";
    public string Username {get; set;} = "";
    public string Password {get; set;} = "";
    public int Port {get; set;}
}