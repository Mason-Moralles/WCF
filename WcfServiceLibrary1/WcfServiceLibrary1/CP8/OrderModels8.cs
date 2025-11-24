using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WcfServiceLibrary1
{
    [DataContract]
    public class OrderItem8
    {
        [DataMember]
        public int ProductId { get; set; }

        [DataMember]
        public string ProductName { get; set; }

        [DataMember]
        public int Quantity { get; set; }
    }

    [DataContract]
    public class Order8
    {
        [DataMember]
        public int OrderId { get; set; }

        [DataMember]
        public string CustomerName { get; set; }

        [DataMember]
        public DateTime CreatedAt { get; set; }

        [DataMember]
        public List<OrderItem8> Items { get; set; } = new List<OrderItem8>();
    }
}
