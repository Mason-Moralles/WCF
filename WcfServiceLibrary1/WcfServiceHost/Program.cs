using System;
using System.ServiceModel;
using WcfServiceLibrary1; // наш сервис из библиотеки

namespace WcfServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            // Базовый адрес сервиса (можно заменить на свой порт/путь)
            Uri baseAddress = new Uri("http://localhost:8733/Design_Time_Addresses/KT1/Service1/");

            using (ServiceHost host = new ServiceHost(typeof(Service1), baseAddress))
            {
                try
                {
                    host.Open();
                    Console.WriteLine("Сервис запущен.");
                    Console.WriteLine("Адрес: " + baseAddress);
                    Console.WriteLine("Нажмите Enter для остановки сервиса...");
                    Console.ReadLine();
                    host.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при запуске сервиса: " + ex.Message);
                    Console.ReadLine();
                }
            }
        }
    }
}
