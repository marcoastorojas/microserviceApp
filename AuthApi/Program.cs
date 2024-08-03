using System.Reflection;
using Authentication.Applicacion;
using Microsoft.AspNetCore.Mvc;
using Shared.Infrastructure.Events.RabbitMq;
using Shared.Infrastructure.DependencyInjection;
using AuthApi.dependencyInjection;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDomainEventsSubscriptions(Assembly.Load("Authentication")); // Nuestro microservicio
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();



var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

//CONFIGURA LA INFRASTRUCTURA DE COLAS
var RabbitMqConfigure = services.GetRequiredService<RabbitMqConfigureInfrastructure>();
RabbitMqConfigure.Configure();


var rabbitMqConsumer = services.GetRequiredService<RabbitMqConsumer>();
rabbitMqConsumer.Consume();



app.MapPost("/", async([FromServices] SignUp signUp) => {
    await signUp.Crear();
});

app.Run();
