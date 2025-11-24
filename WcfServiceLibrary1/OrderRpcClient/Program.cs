using System;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfServiceLibrary1;

namespace OrderRpcClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== КТ8: Order RPC Client ===");

            CallSync().Wait();
            Console.WriteLine();
            CallAsync().Wait();

            Console.WriteLine();
            Console.WriteLine("Готово. Нажмите Enter для выхода...");
            Console.ReadLine();
        }

        private static async Task CallSync()
        {
            Console.WriteLine("=== Синхронные вызовы IOrderService8 ===");

            var binding = new BasicHttpBinding();
            var address = new EndpointAddress("http://localhost:8760/OrderService8");

            var factory = new ChannelFactory<IOrderService8>(binding, address);
            var client = factory.CreateChannel();

            try
            {
                // 1. Получить все заказы
                var orders = client.GetOrders();
                Console.WriteLine($"Всего заказов: {orders.Count}");

                // 2. Добавить новый заказ
                var newOrder = new Order8
                {
                    CustomerName = "Charlie",
                    CreatedAt = DateTime.Now
                };
                newOrder.Items.Add(new OrderItem8
                {
                    ProductId = 500,
                    ProductName = "Headphones",
                    Quantity = 1
                });

                client.AddOrder(newOrder);
                Console.WriteLine("Добавлен новый заказ для Charlie.");

                // 3. Получить по ID
                var order1 = client.GetOrderById(1);
                Console.WriteLine($"Заказ #1: {order1?.CustomerName ?? "не найден"}");
            }
            finally
            {
                try { ((IClientChannel)client).Close(); } catch { ((IClientChannel)client).Abort(); }
                try { factory.Close(); } catch { factory.Abort(); }
            }
        }

        private static async Task CallAsync()
        {
            Console.WriteLine("=== Асинхронные вызовы IOrderService8Async ===");

            var binding = new BasicHttpBinding();
            var address = new EndpointAddress("http://localhost:8760/OrderService8Async");

            var factory = new ChannelFactory<IOrderService8Async>(binding, address);
            var client = factory.CreateChannel();

            try
            {
                var orders = await client.GetOrdersAsync();
                Console.WriteLine($"[Async] Всего заказов: {orders.Count}");

                var order2 = await client.GetOrderByIdAsync(2);
                Console.WriteLine($"[Async] Заказ #2: {order2?.CustomerName ?? "не найден"}");

                var newOrder = new Order8
                {
                    CustomerName = "AsyncUser",
                    CreatedAt = DateTime.Now
                };
                newOrder.Items.Add(new OrderItem8
                {
                    ProductId = 600,
                    ProductName = "Webcam",
                    Quantity = 1
                });

                await client.AddOrderAsync(newOrder);
                Console.WriteLine("[Async] Добавлен заказ для AsyncUser.");

                var updated = await client.GetOrdersAsync();
                Console.WriteLine($"[Async] Заказов после добавления: {updated.Count}");
            }
            finally
            {
                try { ((IClientChannel)client).Close(); } catch { ((IClientChannel)client).Abort(); }
                try { factory.Close(); } catch { factory.Abort(); }
            }
        }
    }
}
