using System.ServiceModel.Channels;

namespace WcfServiceLibrary1
{
    public class CustomMessageEncoderFactory : MessageEncoderFactory
    {
        private readonly MessageEncoder _encoder;
        private readonly MessageVersion _version;

        public CustomMessageEncoderFactory(MessageEncoderFactory innerFactory)
        {
            _encoder = new CustomMessageEncoder(innerFactory.Encoder);
            _version = innerFactory.MessageVersion;
        }

        public override MessageEncoder Encoder => _encoder;

        public override MessageVersion MessageVersion => _version;
    }
}
