
using System.Data;
using Microsoft.Extensions.Options;
using Npgsql;
using Shared.Domain.Events;
using Shared.Infrastructure.Events;
using Shared.Infrastructure.Events.RabbitMq;

namespace AuthApi.dependencyInjection;
public static class Infrastructure
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {


            services.AddSingleton((serviceProvider)=>{
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var options = serviceProvider.GetRequiredService<IOptions<RabbitMqConfigParams>>();
                var RABBITMQ_HOST = configuration["RABBITMQ_HOST"];
                options.Value.HostName = RABBITMQ_HOST ?? options.Value.HostName;
                return new RabbitMqConnection(options);
            });

            services.AddScoped<IDbConnection>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                return new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<DomainEventFailRepository,NpgsqlDomainEventFailRespository>();
            services.AddScoped<EventBus,RabbitMqEventBus>();
            services.AddScoped<RabbitMqConsumer>();

            services.Configure<RabbitMqConfigParams>(configuration.GetSection("RabbitMq"));

            services.AddScoped<RabbitMqConfigureInfrastructure>();

            return services;
        }
}