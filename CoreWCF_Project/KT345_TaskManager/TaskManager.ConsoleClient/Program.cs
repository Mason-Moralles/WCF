using System.ServiceModel;
using TaskManager.Core.Contracts;
using TaskManager.Core.Enums;

Console.WriteLine("=== TASK MANAGER CLIENT ===\n");

var binding = new BasicHttpBinding();
var endpoint = new EndpointAddress("http://localhost:5020/TaskService/basic");
var factory = new ChannelFactory<ITaskManagerService>(binding, endpoint);
var client = factory.CreateChannel();

try
{
    // 1. Аутентификация
    Console.WriteLine("--- Authentication ---");
    var adminToken = client.Authenticate("admin", "pass123");
    Console.WriteLine($"Admin authenticated. Role={adminToken.Role}, Expires={adminToken.Expires:HH:mm:ss}");

    var devToken = client.Authenticate("dev1", "pass123");
    Console.WriteLine($"Dev1 authenticated. Role={devToken.Role}");

    // 2. Получить все проекты
    Console.WriteLine("\n--- Projects ---");
    var projects = client.GetAllProjects(devToken.Token);
    foreach (var p in projects)
        Console.WriteLine($"  Project #{p.Id}: {p.Name} - {p.Description}");

    // 3. Создать проект (только Admin)
    Console.WriteLine("\n--- Create Project (Admin) ---");
    var newProject = client.CreateProject(adminToken.Token, "Backend API", "REST API development");
    Console.WriteLine($"Created: #{newProject.Id} {newProject.Name}");

    // Dev пытается создать проект
    try
    {
        client.CreateProject(devToken.Token, "Test", "Test");
        Console.WriteLine("ERROR: Dev should not create projects!");
    }
    catch (FaultException ex)
    {
        Console.WriteLine($"[Expected] {ex.Message}");
    }

    // 4. Создать задачу
    Console.WriteLine("\n--- Create Task ---");
    var newTask = client.CreateTask(devToken.Token, "Implement login",
        "Add JWT authentication", TaskPriority.High, 1, 2, DateTime.UtcNow.AddDays(5));
    Console.WriteLine($"Created: Task #{newTask.Id} '{newTask.Title}' [{newTask.Priority}] -> {newTask.Status}");

    // 5. Получить задачи по проекту
    Console.WriteLine("\n--- Tasks in Project 1 ---");
    var tasks = client.GetTasksByProject(devToken.Token, 1);
    foreach (var t in tasks)
        Console.WriteLine($"  #{t.Id}: {t.Title} [{t.Priority}] Status={t.Status} Assignee={t.AssigneeId}");

    // 6. Обновить статус задачи
    Console.WriteLine("\n--- Update Task Status ---");
    client.UpdateTaskStatus(devToken.Token, newTask.Id, TaskItemStatus.InProgress);
    var updated = client.GetTask(devToken.Token, newTask.Id);
    Console.WriteLine($"Task #{updated.Id}: {updated.Title} -> {updated.Status}");

    // 7. Получить задачи по исполнителю
    Console.WriteLine("\n--- Tasks for Dev1 (userId=2) ---");
    var devTasks = client.GetTasksByAssignee(devToken.Token, 2);
    foreach (var t in devTasks)
        Console.WriteLine($"  #{t.Id}: {t.Title} [{t.Status}]");

    // 8. Отчёт по нагрузке
    Console.WriteLine("\n--- Workload Report ---");
    var report = client.GetUserWorkload(devToken.Token, 2);
    Console.WriteLine($"User: {report.UserName}");
    Console.WriteLine($"  Total: {report.TotalTasks}, Done: {report.CompletedTasks}, InProgress: {report.InProgressTasks}, Overdue: {report.OverdueTasks}");

    // 9. Удаление задачи (только Admin)
    Console.WriteLine("\n--- Delete Task (Admin) ---");
    var deleted = client.DeleteTask(adminToken.Token, newTask.Id);
    Console.WriteLine($"Deleted task #{newTask.Id}: {deleted}");
}
catch (FaultException ex)
{
    Console.WriteLine($"\nFault: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"\nError: {ex.Message}");
}
finally
{
    ((IClientChannel)client).Close();
}

Console.WriteLine("\nНажмите любую клавишу для выхода...");
Console.ReadKey();
