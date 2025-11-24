using System;
using EmployeeClient.EmployeeRef;

namespace EmployeeClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new EmployeeServiceClient();

            while (true)
            {
                Console.WriteLine("===== Employee Service =====");
                Console.WriteLine("1. Добавить сотрудника");
                Console.WriteLine("2. Удалить сотрудника");
                Console.WriteLine("3. Показать сотрудников");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите: ");

                var cmd = Console.ReadLine();

                switch (cmd)
                {
                    case "1":
                        Console.Write("ID: ");
                        int id = int.Parse(Console.ReadLine());
                        Console.Write("Имя: ");
                        string name = Console.ReadLine();
                        client.AddEmployee(new Employee { Id = id, Name = name });
                        break;

                    case "2":
                        Console.Write("Введите ID для удаления: ");
                        int delId = int.Parse(Console.ReadLine());
                        client.DeleteEmployee(delId);
                        break;

                    case "3":
                        var list = client.GetEmployees();
                        Console.WriteLine("--- Сотрудники ---");
                        foreach (var emp in list)
                            Console.WriteLine($"{emp.Id}: {emp.Name}");
                        break;

                    case "0":
                        client.Close();
                        return;
                }

                Console.WriteLine();
            }
        }
    }
}
