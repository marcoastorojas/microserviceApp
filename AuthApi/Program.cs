using System.Reflection;
using Authentication.Applicacion;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.Events;
using Shared.Infrastrucure.DependencyInjection;
using Shared.Infrastrucure.Events.RabbitMq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDomainEventsSubscriptions(Assembly.Load("Authentication"));
builder.Services.AddSingleton<RabbitMqConnection>();
builder.Services.AddScoped<EventBus,RabbitMqEventBus>();
builder.Services.AddScoped<RabbitMqConsumer>();
builder.Services.AddScoped<SignUp>();
builder.Services.Configure<RabbitMqConfigParams>(builder.Configuration.GetSection("RabbitMq"));





var app = builder.Build();

using var scope = app.Services.CreateScope();

var services = scope.ServiceProvider;
var rabbitMqConsumer = services.GetRequiredService<RabbitMqConsumer>();
rabbitMqConsumer.Consume();

app.MapPost("/", async([FromServices] SignUp signUp) => {
    await signUp.Crear();
});

app.Run();
