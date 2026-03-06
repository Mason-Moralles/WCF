using System.Runtime.Serialization;

namespace TaskManager.Core.Enums;

[DataContract]
public enum TaskPriority
{
    [EnumMember] Low = 0,
    [EnumMember] Medium = 1,
    [EnumMember] High = 2,
    [EnumMember] Critical = 3
}
