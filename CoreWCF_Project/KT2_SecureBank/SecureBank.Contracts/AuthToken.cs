using System.Runtime.Serialization;

namespace SecureBank.Contracts;

[DataContract]
public class AuthToken
{
    [DataMember] public string Token { get; set; } = string.Empty;
    [DataMember] public DateTime Expires { get; set; }
    [DataMember] public string[] Roles { get; set; } = Array.Empty<string>();
}
