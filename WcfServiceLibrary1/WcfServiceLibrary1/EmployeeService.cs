using System.Collections.Generic;
using System.Linq;

namespace WcfServiceLibrary1
{
    public class EmployeeService : IEmployeeService
    {
        private static List<Employee> employees = new List<Employee>();

        public void AddEmployee(Employee employee)
        {
            employees.Add(employee);
        }

        public void DeleteEmployee(int id)
        {
            employees.RemoveAll(e => e.Id == id);
        }

        public List<Employee> GetEmployees()
        {
            return employees.ToList();
        }
    }
}
