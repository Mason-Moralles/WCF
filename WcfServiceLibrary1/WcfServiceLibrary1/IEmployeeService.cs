using System.Collections.Generic;
using System.ServiceModel;

namespace WcfServiceLibrary1
{
    [ServiceContract]
    public interface IEmployeeService
    {
        [OperationContract]
        void AddEmployee(Employee employee);

        [OperationContract]
        void DeleteEmployee(int id);

        [OperationContract]
        List<Employee> GetEmployees();
    }
}
