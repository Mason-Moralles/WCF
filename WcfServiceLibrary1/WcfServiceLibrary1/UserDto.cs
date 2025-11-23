using System.Runtime.Serialization;

namespace WcfServiceLibrary1
{
    [DataContract]
    public class UserDto
    {
        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}
