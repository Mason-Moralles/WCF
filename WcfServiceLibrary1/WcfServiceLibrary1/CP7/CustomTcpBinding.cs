using System.ServiceModel.Channels;
using WcfServiceLibrary1;

namespace WcfServiceLibrary1
{
    public class CustomTcpBinding : Binding
    {
        public override string Scheme => "net.tcp";

        public override BindingElementCollection CreateBindingElements()
        {
            var elements = new BindingElementCollection();

            // Сначала наш кастомный encoder
            elements.Add(new CustomMessageEncodingBindingElement());

            // Затем стандартный TCP-транспорт
            elements.Add(new TcpTransportBindingElement());

            return elements;
        }
    }
}
