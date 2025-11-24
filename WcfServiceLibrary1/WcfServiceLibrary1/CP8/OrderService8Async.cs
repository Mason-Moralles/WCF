using System.Collections.Generic;
using System.Threading.Tasks;

namespace WcfServiceLibrary1
{
    public class OrderService8Async : IOrderService8Async
    {
        public Task<List<Order8>> GetOrdersAsync()
        {
            // имитация асинхронной работы
            return Task.FromResult(OrderStore8.GetAll());
        }

        public Task<Order8> GetOrderByIdAsync(int id)
        {
            return Task.FromResult(OrderStore8.GetById(id));
        }

        public Task AddOrderAsync(Order8 order)
        {
            OrderStore8.Add(order);
            return Task.CompletedTask;
        }
    }
}
