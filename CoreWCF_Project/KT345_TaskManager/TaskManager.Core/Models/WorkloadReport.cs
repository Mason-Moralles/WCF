using System.Runtime.Serialization;

namespace TaskManager.Core.Models;

[DataContract]
public class WorkloadReport
{
    [DataMember] public int UserId { get; set; }
    [DataMember] public string UserName { get; set; } = string.Empty;
    [DataMember] public int TotalTasks { get; set; }
    [DataMember] public int CompletedTasks { get; set; }
    [DataMember] public int InProgressTasks { get; set; }
    [DataMember] public int OverdueTasks { get; set; }
}
