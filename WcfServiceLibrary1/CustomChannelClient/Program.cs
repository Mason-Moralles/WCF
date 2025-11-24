using System;
using System.ServiceModel;
using WcfServiceLibrary1;

namespace CustomChannelClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var binding = new CustomTcpBinding();
            var address = new EndpointAddress("net.tcp://localhost:9001/CustomChannelService");

            var factory = new ChannelFactory<ICustomChannelService>(binding, address);
            var client = factory.CreateChannel();

            Console.WriteLine("Введите сообщение для отправки (или пустую строку для выхода):");

            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    break;

                try
                {
                    var response = client.Echo(input);
                    Console.WriteLine("Ответ сервиса: " + response);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка: " + ex.Message);
                }
            }

            ((IClientChannel)client).Close();
            factory.Close();
        }
    }
}
