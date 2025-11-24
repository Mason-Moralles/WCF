using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceLibrary1
{
    [ServiceContract]
    public interface ICustomChannelService
    {
        [OperationContract]
        string Echo(string message);
    }
}
