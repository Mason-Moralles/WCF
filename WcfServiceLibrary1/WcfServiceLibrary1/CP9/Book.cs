using System;
using System.Runtime.Serialization;

namespace WcfServiceLibrary1
{
    [DataContract]
    public class Book
    {
        [DataMember]
        public int BookId { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Author { get; set; }

        [DataMember]
        public DateTime PublishedDate { get; set; }
    }
}
