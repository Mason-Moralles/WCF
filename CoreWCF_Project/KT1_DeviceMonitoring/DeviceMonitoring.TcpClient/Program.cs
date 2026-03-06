using System.Diagnostics;
using System.ServiceModel;
using DeviceMonitoring.Contracts;

Console.WriteLine("=== TCP CLIENT (NetTcpBinding) ===\n");

var binding = new NetTcpBinding(SecurityMode.None);
var endpoint = new EndpointAddress("net.tcp://localhost:8090/DeviceService/nettcp");
var factory = new ChannelFactory<IDeviceManager>(binding, endpoint);
var client = factory.CreateChannel();

try
{
    // 1. Получить устройство
    var sw = Stopwatch.StartNew();
    var device = client.GetDevice(3);
    sw.Stop();
    Console.WriteLine($"[GetDevice(3)] {device.Name}, Online={device.IsOnline} ({sw.ElapsedMilliseconds} ms)");

    // 2. Пинг устройства
    sw.Restart();
    var pingResult = client.PingDevice(3);
    sw.Stop();
    Console.WriteLine($"[PingDevice(3)] Result: {pingResult} ({sw.ElapsedMilliseconds} ms)");

    // 3. Статистика
    sw.Restart();
    var stats = client.GetServiceStats();
    sw.Stop();
    Console.WriteLine($"[Stats] {stats} ({sw.ElapsedMilliseconds} ms)");

    // 4. Сравнение производительности: 100 вызовов
    Console.WriteLine("\n=== PERFORMANCE TEST (100 calls GetDevice) ===");
    sw.Restart();
    for (int i = 0; i < 100; i++)
    {
        client.GetDevice(1);
    }
    sw.Stop();

    Console.WriteLine($"100 TCP calls: {sw.ElapsedMilliseconds} ms");
    Console.WriteLine($"Average: {sw.ElapsedMilliseconds / 100.0:F2} ms per call");
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
