using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceLibrary1
{
    public class OrderService : IOrderService
    {
        public List<OrderStatus> GetOrderStatuses()
        {
            return new List<OrderStatus>
            {
                new OrderStatus { Status = "Processing", Count = 5 },
                new OrderStatus { Status = "Shipped",   Count = 10 },
                new OrderStatus { Status = "Delivered", Count = 15 },
            };
        }
    }
}
