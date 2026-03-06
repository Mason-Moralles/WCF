using System.Runtime.Serialization;

namespace TaskManager.Core.Models;

[DataContract]
public class UserInfo
{
    [DataMember] public int Id { get; set; }
    [DataMember] public string Username { get; set; } = string.Empty;
    [DataMember] public string FullName { get; set; } = string.Empty;
    [DataMember] public string Role { get; set; } = string.Empty;
}
