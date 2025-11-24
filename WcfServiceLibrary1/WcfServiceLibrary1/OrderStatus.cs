using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace WcfServiceLibrary1
{
    [DataContract]
    public class OrderStatus
    {
        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public int Count { get; set; }
    }
}
