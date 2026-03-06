using System.Runtime.Serialization;

namespace DeviceMonitoring.Contracts;

[DataContract]
public class DeviceInfo
{
    [DataMember] public int Id { get; set; }
    [DataMember] public string Name { get; set; } = string.Empty;
    [DataMember] public DateTime LastPing { get; set; }
    [DataMember] public bool IsOnline { get; set; }
}
