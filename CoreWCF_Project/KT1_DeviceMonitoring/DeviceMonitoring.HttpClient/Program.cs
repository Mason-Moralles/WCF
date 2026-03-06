using System.ServiceModel;
using DeviceMonitoring.Contracts;

Console.WriteLine("=== HTTP CLIENT (BasicHttpBinding) ===\n");

var binding = new BasicHttpBinding();
var endpoint = new EndpointAddress("http://localhost:5000/DeviceService/basic");
var factory = new ChannelFactory<IDeviceManager>(binding, endpoint);
var client = factory.CreateChannel();

try
{
    // 1. Получить статистику
    var stats = client.GetServiceStats();
    Console.WriteLine($"[Stats] {stats}");

    // 2. Получить все устройства
    var devices = client.GetAllDevices();
    Console.WriteLine($"\n[All Devices] Total: {devices.Count}");
    foreach (var d in devices)
    {
        Console.WriteLine($"  ID={d.Id}, Name={d.Name}, Online={d.IsOnline}, LastPing={d.LastPing:HH:mm:ss}");
    }

    // 3. Получить конкретное устройство
    var device = client.GetDevice(1);
    Console.WriteLine($"\n[GetDevice(1)] {device.Name}, Online={device.IsOnline}");

    // 4. Пинг устройства
    var pingResult = client.PingDevice(2);
    Console.WriteLine($"\n[PingDevice(2)] Result: {pingResult}");

    // 5. Проверить обновлённую статистику
    stats = client.GetServiceStats();
    Console.WriteLine($"\n[Final Stats] {stats}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
finally
{
    ((IClientChannel)client).Close();
}

Console.WriteLine("\nНажмите любую клавишу для выхода...");
Console.ReadKey();
