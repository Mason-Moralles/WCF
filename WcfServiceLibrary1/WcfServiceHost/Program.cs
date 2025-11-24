using System;
using System.ServiceModel;
using WcfServiceLibrary1;
using System.ServiceModel.Security;

namespace WcfServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var host = new ServiceHost(typeof(Service1)))
            using (var empHost = new ServiceHost(typeof(EmployeeService)))
            using (var hostOrders = new ServiceHost(typeof(OrderService)))
            using (var hostSecurity = new ServiceHost(typeof(SecurityService)))
            {
                host.Open();
                empHost.Open();
                hostOrders.Open();
                hostSecurity.Open();

                Console.WriteLine("=== Сервисы запущены ===");

                foreach (var h in new[] { host, empHost, hostOrders, hostSecurity })
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

                hostSecurity.Close();
                hostOrders.Close();
                empHost.Close();
                host.Close();
            }
        }
    }
}
