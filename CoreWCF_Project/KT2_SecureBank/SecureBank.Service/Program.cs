using CoreWCF;
using CoreWCF.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SecureBank.Contracts;
using SecureBank.Service.Data;
using SecureBank.Service.Models;
using SecureBank.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Entity Framework InMemory
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseInMemoryDatabase("SecureBankDb"));

// ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT + сервис
builder.Services.AddSingleton<JwtService>();

// CoreWCF
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<BankService>();

var app = builder.Build();

// Сидирование пользователей
await SeedUsersAsync(app.Services);

// CoreWCF-эндпоинты
app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<BankService>();

    var httpBinding = new BasicHttpBinding(CoreWCF.Channels.BasicHttpSecurityMode.None);
    serviceBuilder.AddServiceEndpoint<BankService, IBankService>(
        httpBinding,
        "/BankService/basic");

    var metadata = app.Services.GetRequiredService<CoreWCF.Description.ServiceMetadataBehavior>();
    metadata.HttpGetEnabled = true;
});

// REST-эндпоинт для получения JWT (удобный для тестирования)
app.MapPost("/api/auth/login", async (LoginRequest req, JwtService jwt) =>
{
    var (roles, accountNum) = req.Username?.ToLower() switch
    {
        "client" when req.Password == "pass123" => (new[] { "Client" }, "ACC001"),
        "operator" when req.Password == "pass123" => (new[] { "Operator" }, "ACC002"),
        "admin" when req.Password == "pass123" => (new[] { "Administrator" }, "ACC003"),
        _ => (Array.Empty<string>(), "")
    };

    if (roles.Length == 0)
        return Results.Unauthorized();

    var token = jwt.GenerateToken(req.Username!, Guid.NewGuid().ToString(), roles, accountNum);
    return Results.Ok(new { Token = token });
});

app.MapGet("/health", () =>
    "SecureBank Service running.\n" +
    "SOAP: http://localhost:5010/BankService/basic\n" +
    "Auth: POST http://localhost:5010/api/auth/login\n" +
    "Users: client/pass123, operator/pass123, admin/pass123");

app.Run();

static async Task SeedUsersAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = ["Client", "Operator", "Administrator"];
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    var users = new[]
    {
        ("client", "Client", "ACC001", "Client User"),
        ("operator", "Operator", "ACC002", "Operator User"),
        ("admin", "Administrator", "ACC003", "Admin User")
    };

    foreach (var (name, role, acc, fullName) in users)
    {
        if (await userManager.FindByNameAsync(name) == null)
        {
            var user = new ApplicationUser
            {
                UserName = name,
                FullName = fullName,
                AccountNumber = acc,
                Email = $"{name}@bank.local"
            };
            await userManager.CreateAsync(user, "Pass123!");
            await userManager.AddToRoleAsync(user, role);
        }
    }
}

public record LoginRequest(string? Username, string? Password);
