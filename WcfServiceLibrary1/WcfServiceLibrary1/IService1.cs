using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace WcfServiceLibrary1
{
    [ServiceContract]
    public interface IService1
    {
        // Получить всех пользователей
        [OperationContract]
        List<UserDto> GetUsers();
    }
}
