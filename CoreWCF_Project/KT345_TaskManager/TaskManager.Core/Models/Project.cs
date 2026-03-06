using System.Runtime.Serialization;

namespace TaskManager.Core.Models;

[DataContract]
public class Project
{
    [DataMember] public int Id { get; set; }
    [DataMember] public string Name { get; set; } = string.Empty;
    [DataMember] public string Description { get; set; } = string.Empty;
    [DataMember] public DateTime CreatedAt { get; set; }
    [DataMember] public int OwnerId { get; set; }
}
