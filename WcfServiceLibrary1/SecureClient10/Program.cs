using System;
using System.ServiceModel;
using WcfServiceLibrary1;

namespace SecureClient10
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== КТ10: Secure RPC Client ===");

            // 1. Создаём защищённую привязку
            var binding = new WSHttpBinding(SecurityMode.Message);
            binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;

            // 2. Адрес сервиса
            var endpoint = new EndpointAddress("http://localhost:8780/SecureService");

            // 3. Создание канала RPC
            var channelFactory = new ChannelFactory<ISecureService>(binding, endpoint);

            var client = channelFactory.CreateChannel();

            try
            {
                Console.WriteLine(client.GetSecureData());
                Console.WriteLine(client.GetUserName());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка безопасности: " + ex.Message);
            }

            Console.WriteLine("Готово! Нажмите Enter...");
            Console.ReadLine();
        }
    }
}
