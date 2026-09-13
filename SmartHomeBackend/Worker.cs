using Microsoft.AspNetCore.SignalR;
using MQTTnet;

namespace SmartHomeBackend;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IHubContext<SmartHomeHub> _hubContext;

    public Worker(ILogger<Worker> logger, IHubContext<SmartHomeHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var factory = new MqttClientFactory();

        using var mqttClient = factory.CreateMqttClient();

        var options = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost", 1883)
            .Build();

        // Handle incoming MQTT messages
        mqttClient.ApplicationMessageReceivedAsync +=async e =>
        {
            var topic = e.ApplicationMessage.Topic;

            var message =
                e.ApplicationMessage.ConvertPayloadToString();

            _logger.LogInformation(
                "Received | Topic: {Topic} | Message: {Message}",
                topic,
                message);
            
            
            await _hubContext.Clients.All.SendAsync(
                "ReceiveTemperature",
                topic,
                message);

          
        };

        // Connect to MQTT Broker
        _logger.LogInformation(
            "Connecting to MQTT Broker...");

        await mqttClient.ConnectAsync(
            options,
            stoppingToken);

        _logger.LogInformation("Connected!");

        // Subscribe to temperature topic
        await mqttClient.SubscribeAsync(
            "home/room1/temperature",
            cancellationToken: stoppingToken);

        _logger.LogInformation(
            "Subscribed to home/room/temperature");

        // Keep the Worker running
        await Task.Delay(
            Timeout.Infinite,
            stoppingToken);
    }
}