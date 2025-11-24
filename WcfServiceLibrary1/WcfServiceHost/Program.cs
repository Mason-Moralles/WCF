using System;
using System.ServiceModel;
using System.ServiceModel.Security;
using WcfServiceLibrary1;

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
            using (var customHost = new ServiceHost(typeof(CustomChannelService),
                   new Uri("net.tcp://localhost:9001/CustomChannelService")))
            // КТ8: новые хосты
            using (var orderHost8 = new ServiceHost(
                       typeof(OrderService8),
                       new Uri("http://localhost:8760/OrderService8")))
            using (var orderHost8Async = new ServiceHost(
                       typeof(OrderService8Async),
                       new Uri("http://localhost:8760/OrderService8Async")))
            {
                // КТ7: добавляем endpoint с CustomTcpBinding
                customHost.AddServiceEndpoint(
                    typeof(ICustomChannelService),
                    new CustomTcpBinding(),
                    "");

                host.Open();
                empHost.Open();
                hostOrders.Open();
                hostSecurity.Open();
                customHost.Open();
                orderHost8.Open();
                orderHost8Async.Open();

                Console.WriteLine("=== Сервисы запущены ===");

                foreach (var h in new[] { host, empHost, hostOrders, hostSecurity, customHost, orderHost8, orderHost8Async})
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

                orderHost8Async.Close();
                orderHost8.Close();
                customHost.Close();
                hostSecurity.Close();
                hostOrders.Close();
                empHost.Close();
                host.Close();
            }
        }
    }
}
