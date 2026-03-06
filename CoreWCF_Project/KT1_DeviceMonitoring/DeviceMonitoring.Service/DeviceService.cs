using CoreWCF;
using DeviceMonitoring.Contracts;
using System.Collections.Concurrent;

namespace DeviceMonitoring.Service;

public class DeviceService : IDeviceManager
{
    private readonly ConcurrentDictionary<int, DeviceInfo> _devices;
    private int _httpCallCount;
    private int _tcpCallCount;

    public DeviceService()
    {
        _devices = new ConcurrentDictionary<int, DeviceInfo>();

        for (int i = 1; i <= 10; i++)
        {
            _devices[i] = new DeviceInfo
            {
                Id = i,
                Name = $"Device_{i}",
                LastPing = DateTime.UtcNow.AddMinutes(-i),
                IsOnline = i % 2 == 0
            };
        }
    }

    public List<DeviceInfo> GetAllDevices()
    {
        TrackTransport();
        return _devices.Values.ToList();
    }

    public DeviceInfo GetDevice(int id)
    {
        TrackTransport();
        return _devices.TryGetValue(id, out var device)
            ? device
            : new DeviceInfo { Id = -1, Name = "Not Found" };
    }

    public bool PingDevice(int id)
    {
        TrackTransport();

        if (!_devices.TryGetValue(id, out var device))
            return false;

        device.LastPing = DateTime.UtcNow;
        device.IsOnline = true;
        return true;
    }

    public string GetServiceStats()
    {
        TrackTransport();
        return $"HTTP calls: {_httpCallCount}, TCP calls: {_tcpCallCount}";
    }

    private void TrackTransport()
    {
        var context = OperationContext.Current;
        if (context == null) return;

        var scheme = context.IncomingMessageProperties?.Via?.Scheme;
        if (scheme == "http" || scheme == "https")
            Interlocked.Increment(ref _httpCallCount);
        else
            Interlocked.Increment(ref _tcpCallCount);
    }
}
