using System;
using System.IO;
using System.ServiceModel.Channels;
using System.Text;

namespace WcfServiceLibrary1
{
    public class CustomMessageEncoder : MessageEncoder
    {
        private readonly MessageEncoder innerEncoder;

        public CustomMessageEncoder(MessageEncoder innerEncoder)
        {
            this.innerEncoder = innerEncoder ?? throw new ArgumentNullException(nameof(innerEncoder));
        }

        public override string ContentType => innerEncoder.ContentType;
        public override string MediaType => innerEncoder.MediaType;
        public override MessageVersion MessageVersion => innerEncoder.MessageVersion;

        public override bool IsContentTypeSupported(string contentType)
        {
            return innerEncoder.IsContentTypeSupported(contentType);
        }

        public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
        {
            // Логируем входящее сообщение (как текст)
            var msgBytes = new byte[buffer.Count];
            Array.Copy(buffer.Array, buffer.Offset, msgBytes, 0, buffer.Count);
            string text = Encoding.UTF8.GetString(msgBytes);
            Console.WriteLine("[Encoder] ReadMessage (buffer):");
            Console.WriteLine(text);

            return innerEncoder.ReadMessage(buffer, bufferManager, contentType);
        }

        public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
        {
            // Логируем входящее сообщение из Stream
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                var bytes = ms.ToArray();
                string text = Encoding.UTF8.GetString(bytes);
                Console.WriteLine("[Encoder] ReadMessage (stream):");
                Console.WriteLine(text);

                ms.Position = 0;
                return innerEncoder.ReadMessage(ms, maxSizeOfHeaders, contentType);
            }
        }

        public override void WriteMessage(Message message, Stream stream)
        {
            // Пишем во временный stream, чтобы залогировать
            using (var ms = new MemoryStream())
            {
                innerEncoder.WriteMessage(message, ms);
                var bytes = ms.ToArray();
                string text = Encoding.UTF8.GetString(bytes);
                Console.WriteLine("[Encoder] WriteMessage (stream):");
                Console.WriteLine(text);

                stream.Write(bytes, 0, bytes.Length);
            }
        }

        public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
        {
            var segment = innerEncoder.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);

            var msgBytes = new byte[segment.Count];
            Array.Copy(segment.Array, segment.Offset, msgBytes, 0, segment.Count);
            string text = Encoding.UTF8.GetString(msgBytes);
            Console.WriteLine("[Encoder] WriteMessage (buffer):");
            Console.WriteLine(text);

            return segment;
        }
    }
}
