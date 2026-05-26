using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/health");

Task.Run(() =>
{
    var factory = new ConnectionFactory()
    {
        HostName = "rabbitmq"
    };

    var connection = factory.CreateConnection();
    var channel = connection.CreateModel();

    channel.QueueDeclare(
        queue: "orders",
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: null);

    var consumer = new EventingBasicConsumer(channel);

    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        Console.WriteLine($"[x] Notification received: {message}");
    };

    channel.BasicConsume(
        queue: "orders",
        autoAck: true,
        consumer: consumer);

    Console.WriteLine("RabbitMQ consumer started");
});

app.MapGet("/", () => "Notification Service");

app.Run();