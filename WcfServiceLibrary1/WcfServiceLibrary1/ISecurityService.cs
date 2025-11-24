using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace WcfServiceLibrary1
{
    [ServiceContract]
    public interface ISecurityService
    {
        [OperationContract]
        string GetPublicInfo();

        [OperationContract]
        string GetManagerSecret();   // доступно только менеджерам
    }
}
