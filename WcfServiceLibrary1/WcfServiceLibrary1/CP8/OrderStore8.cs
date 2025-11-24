using System;
using System.Collections.Generic;
using System.Linq;

namespace WcfServiceLibrary1
{
    public static class OrderStore8
    {
        // Одна общая коллекция заказов для sync и async сервисов
        private static readonly List<Order8> _orders = new List<Order8>
        {
            new Order8
            {
                OrderId = 1,
                CustomerName = "Alice",
                CreatedAt = DateTime.Now.AddDays(-2),
                Items =
                {
                    new OrderItem8 { ProductId = 100, ProductName = "Keyboard", Quantity = 1 },
                    new OrderItem8 { ProductId = 200, ProductName = "Mouse", Quantity = 2 },
                }
            },
            new Order8
            {
                OrderId = 2,
                CustomerName = "Bob",
                CreatedAt = DateTime.Now.AddDays(-1),
                Items =
                {
                    new OrderItem8 { ProductId = 300, ProductName = "Monitor", Quantity = 1 },
                }
            }
        };

        private static int _nextId = 3;

        public static List<Order8> GetAll() => new List<Order8>(_orders);

        public static Order8 GetById(int id) => _orders.FirstOrDefault(o => o.OrderId == id);

        public static void Add(Order8 order)
        {
            if (order.OrderId == 0)
                order.OrderId = _nextId++;

            _orders.Add(order);
        }
    }
}
