using System.Runtime.Serialization;

namespace TaskManager.Core.Enums;

[DataContract]
public enum TaskItemStatus
{
    [EnumMember] New = 0,
    [EnumMember] InProgress = 1,
    [EnumMember] Done = 2,
    [EnumMember] Cancelled = 3
}
