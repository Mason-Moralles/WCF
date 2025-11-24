using System;
using System.ServiceModel;
using WcfServiceLibrary1;

namespace WcfServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(Service1)))
            using (ServiceHost empHost = new ServiceHost(typeof(EmployeeService)))
            using (var hostOrders = new ServiceHost(typeof(OrderService)))
            {
                host.Open();
                empHost.Open();
                hostOrders.Open();

                Console.WriteLine("=== Сервисы запущены ===");

                foreach (var h in new[] { host, empHost, hostOrders})
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

                hostOrders.Close();
                empHost.Close();
                host.Close();
            }
        }
    }
}
