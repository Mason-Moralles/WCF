using CoreWCF;
using CoreWCF.Configuration;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TaskManager.API.Services;
using TaskManager.Application.Services;
using TaskManager.Core.Contracts;
using TaskManager.Core.Interfaces;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Repositories;

// Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/taskmanager-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// EF Core InMemory
builder.Services.AddDbContext<TaskDbContext>(opt =>
    opt.UseInMemoryDatabase("TaskManagerDb"));

// DI — репозитории
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// DI — сервисы
builder.Services.AddScoped<TaskAppService>();
builder.Services.AddSingleton<JwtService>();

// CoreWCF
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddTransient<TaskManagerWcfService>();

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
    db.Database.EnsureCreated();
}

// CoreWCF endpoints
app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<TaskManagerWcfService>();

    var httpBinding = new BasicHttpBinding(CoreWCF.Channels.BasicHttpSecurityMode.None);
    serviceBuilder.AddServiceEndpoint<TaskManagerWcfService, ITaskManagerService>(
        httpBinding,
        "/TaskService/basic");

    var metadata = app.Services.GetRequiredService<CoreWCF.Description.ServiceMetadataBehavior>();
    metadata.HttpGetEnabled = true;
});

app.MapGet("/health", () =>
    "TaskManager Service running.\n" +
    "SOAP: http://localhost:5020/TaskService/basic\n" +
    "WSDL: http://localhost:5020/TaskService/basic?wsdl\n" +
    "Users: admin/pass123, dev1/pass123, dev2/pass123");

app.Run();
