using System.ServiceModel;

namespace WcfServiceLibrary1
{
    [ServiceContract]
    public interface ISecureService
    {
        [OperationContract]
        string GetSecureData();

        [OperationContract]
        string GetUserName(); // Покажем, что Windows-аутентификация работает
    }
}
