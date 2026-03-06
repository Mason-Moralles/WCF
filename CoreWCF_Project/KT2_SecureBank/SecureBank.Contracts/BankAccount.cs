using System.Runtime.Serialization;

namespace SecureBank.Contracts;

[DataContract]
public class BankAccount
{
    [DataMember] public string AccountNumber { get; set; } = string.Empty;
    [DataMember] public decimal Balance { get; set; }
    [DataMember] public string Currency { get; set; } = "USD";
    [DataMember] public bool IsBlocked { get; set; }
}
