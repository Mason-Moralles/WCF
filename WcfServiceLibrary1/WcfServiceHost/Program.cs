using System;
using System.ServiceModel;
using WcfServiceLibrary1; // наш сервис из библиотеки

namespace WcfServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(Service1)))
            {
                host.Open();

                Console.WriteLine("Сервис запущен!");

                // Выводим базовые адреса (baseAddresses)
                foreach (var baseAddress in host.BaseAddresses)
                {
                    Console.WriteLine("Базовый адрес: " + baseAddress);
                }

                // Выводим все endpoints (адрес + контракт)
                Console.WriteLine("\nEndpoints:");
                foreach (var ep in host.Description.Endpoints)
                {
                    Console.WriteLine($" - {ep.Address.Uri} (contract: {ep.Contract.Name})");
                }

                Console.WriteLine("\nПерейдите в браузере по адресу:");
                Console.WriteLine(host.BaseAddresses[0] + "?wsdl");

                Console.WriteLine("\nНажмите Enter для остановки сервиса...");
                Console.ReadLine();

                host.Close();
            }
        }
    }
}
