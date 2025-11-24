using System;
using System.ServiceModel.Channels;
using System.Text;
using WcfServiceLibrary1;

namespace WcfServiceLibrary1
{
    public class CustomMessageEncodingBindingElement : MessageEncodingBindingElement
    {
        private readonly MessageEncodingBindingElement innerEncodingElement;

        public CustomMessageEncodingBindingElement()
            : this(new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, Encoding.UTF8))
        {
        }

        public CustomMessageEncodingBindingElement(MessageEncodingBindingElement inner)
        {
            innerEncodingElement = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public override MessageVersion MessageVersion
        {
            get => innerEncodingElement.MessageVersion;
            set => innerEncodingElement.MessageVersion = value;
        }

        public override MessageEncoderFactory CreateMessageEncoderFactory()
        {
            var innerFactory = innerEncodingElement.CreateMessageEncoderFactory();
            return new CustomMessageEncoderFactory(innerFactory);
        }

        public override BindingElement Clone()
        {
            return new CustomMessageEncodingBindingElement((MessageEncodingBindingElement)innerEncodingElement.Clone());
        }

        public override T GetProperty<T>(BindingContext context)
        {
            return innerEncodingElement.GetProperty<T>(context);
        }
    }
}
