using MQTTnet;

var factory = new MqttClientFactory();

using var mqttClient = factory.CreateMqttClient();

var options = new MqttClientOptionsBuilder()
    .WithTcpServer("localhost", 1883)
    .Build();

Console.WriteLine("Connecting to MQTT Broker...");

await mqttClient.ConnectAsync(options);

Console.WriteLine("Connected!");

while (true)
{
    var temperature = Random.Shared.Next(20, 33);

    var message = new MqttApplicationMessageBuilder()
        .WithTopic("home/room1/temperature")
        .WithPayload(temperature.ToString())
        .Build();

    await mqttClient.PublishAsync(message);

    Console.WriteLine(
        $"Published temperature: {temperature}°C");

    await Task.Delay(5000);
}