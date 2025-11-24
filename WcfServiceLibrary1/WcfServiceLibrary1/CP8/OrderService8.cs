using System.Collections.Generic;

namespace WcfServiceLibrary1
{
    public class OrderService8 : IOrderService8
    {
        public List<Order8> GetOrders()
        {
            return OrderStore8.GetAll();
        }

        public Order8 GetOrderById(int id)
        {
            return OrderStore8.GetById(id);
        }

        public void AddOrder(Order8 order)
        {
            OrderStore8.Add(order);
        }
    }
}
