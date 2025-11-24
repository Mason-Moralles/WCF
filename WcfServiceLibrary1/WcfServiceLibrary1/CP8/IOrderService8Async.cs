using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace WcfServiceLibrary1
{
    [ServiceContract]
    public interface IOrderService8Async
    {
        [OperationContract]
        Task<List<Order8>> GetOrdersAsync();

        [OperationContract]
        Task<Order8> GetOrderByIdAsync(int id);

        [OperationContract]
        Task AddOrderAsync(Order8 order);
    }
}
