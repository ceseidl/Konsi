using Konsi.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<RabbitMQConsumer>();

var host = builder.Build();
host.Run();
