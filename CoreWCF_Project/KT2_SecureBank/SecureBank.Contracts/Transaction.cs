using System.Runtime.Serialization;

namespace SecureBank.Contracts;

[DataContract]
public class Transaction
{
    [DataMember] public int Id { get; set; }
    [DataMember] public string FromAccount { get; set; } = string.Empty;
    [DataMember] public string ToAccount { get; set; } = string.Empty;
    [DataMember] public decimal Amount { get; set; }
    [DataMember] public DateTime Timestamp { get; set; }
    [DataMember] public string Description { get; set; } = string.Empty;
}
