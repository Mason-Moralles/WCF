using System.Collections.Generic;
using System.ServiceModel;

namespace WcfServiceLibrary1
{
    [ServiceContract]
    public interface IOrderService8
    {
        [OperationContract]
        List<Order8> GetOrders();

        [OperationContract]
        Order8 GetOrderById(int id);

        [OperationContract]
        void AddOrder(Order8 order);
    }
}
