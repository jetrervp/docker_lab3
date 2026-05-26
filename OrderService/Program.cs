using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(sp =>
{
    var factory = new ConnectionFactory()
    {
        HostName = "rabbitmq"
    };

    return factory.CreateConnection();
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapHealthChecks("/health");

app.MapPost("/orders", (IConnection connection, Order order) =>
{
    using var channel = connection.CreateModel();

    channel.QueueDeclare(
        queue: "orders",
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: null);

    var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(order));

    channel.BasicPublish(
        exchange: "",
        routingKey: "orders",
        basicProperties: null,
        body: body);

    return Results.Ok(new
    {
        Message = "Order created",
        Order = order
    });
});

app.Run();

record Order(string ProductName, int Quantity);