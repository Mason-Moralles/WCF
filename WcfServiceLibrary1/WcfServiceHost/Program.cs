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
            using (ServiceHost empHost = new ServiceHost(typeof(EmployeeService)))
            {
                host.Open();
                empHost.Open();

                Console.WriteLine("=== Сервисы запущены ===");

                foreach (var h in new[] { host, empHost })
                {
                    foreach (var ba in h.BaseAddresses)
                        Console.WriteLine($"Базовый адрес: {ba}");

                    Console.WriteLine("Endpoints:");
                    foreach (var ep in h.Description.Endpoints)
                        Console.WriteLine($" - {ep.Address.Uri} ({ep.Contract.Name})");

                    Console.WriteLine();
                }

                Console.WriteLine("Нажмите Enter для остановки...");
                Console.ReadLine();

                empHost.Close();
                host.Close();
            }
        }
    }
}
