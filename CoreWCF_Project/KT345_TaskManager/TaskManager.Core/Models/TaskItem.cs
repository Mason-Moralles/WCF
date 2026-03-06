using System.Runtime.Serialization;
using TaskManager.Core.Enums;

namespace TaskManager.Core.Models;

[DataContract]
public class TaskItem
{
    [DataMember] public int Id { get; set; }
    [DataMember] public string Title { get; set; } = string.Empty;
    [DataMember] public string Description { get; set; } = string.Empty;
    [DataMember] public TaskPriority Priority { get; set; }
    [DataMember] public TaskItemStatus Status { get; set; }
    [DataMember] public int ProjectId { get; set; }
    [DataMember] public int AssigneeId { get; set; }
    [DataMember] public DateTime CreatedAt { get; set; }
    [DataMember] public DateTime? DueDate { get; set; }
}
