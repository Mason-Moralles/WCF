using System.Collections.Concurrent;
using System.Security.Claims;
using CoreWCF;
using SecureBank.Contracts;

namespace SecureBank.Service.Services;

public class BankService : IBankService
{
    private readonly ILogger<BankService> _logger;
    private readonly JwtService _jwt;

    private static readonly ConcurrentDictionary<string, BankAccount> Accounts = new()
    {
        ["ACC001"] = new BankAccount { AccountNumber = "ACC001", Balance = 10000m, Currency = "USD", IsBlocked = false },
        ["ACC002"] = new BankAccount { AccountNumber = "ACC002", Balance = 5000m, Currency = "EUR", IsBlocked = false },
        ["ACC003"] = new BankAccount { AccountNumber = "ACC003", Balance = 15000m, Currency = "GBP", IsBlocked = true }
    };

    private static readonly List<Transaction> Transactions = new()
    {
        new() { Id = 1, FromAccount = "ACC001", ToAccount = "ACC002", Amount = 100, Timestamp = DateTime.UtcNow.AddDays(-1), Description = "Transfer" },
        new() { Id = 2, FromAccount = "ACC002", ToAccount = "ACC001", Amount = 500, Timestamp = DateTime.UtcNow.AddDays(-2), Description = "Salary" },
        new() { Id = 3, FromAccount = "ACC001", ToAccount = "ACC003", Amount = 250, Timestamp = DateTime.UtcNow.AddDays(-5), Description = "Payment" }
    };

    public BankService(ILogger<BankService> logger, JwtService jwt)
    {
        _logger = logger;
        _jwt = jwt;
    }

    public string GetServiceInfo()
    {
        _logger.LogInformation("GetServiceInfo called (public)");
        return "Secure Bank Service v1.0 — JWT authentication required for most operations.";
    }

    public BankAccount GetMyAccount(string token)
    {
        var user = Authorize(token);
        var accountNum = user.FindFirst("AccountNumber")?.Value
            ?? throw new FaultException("Account not linked to user.");

        _logger.LogInformation("User {User} accessed account {Account}", user.Identity?.Name, accountNum);

        return Accounts.TryGetValue(accountNum, out var acc)
            ? acc
            : throw new FaultException($"Account {accountNum} not found.");
    }

    public List<BankAccount> GetAllAccounts(string token)
    {
        var user = Authorize(token, "Operator", "Administrator");
        _logger.LogInformation("User {User} accessed all accounts", user.Identity?.Name);
        return Accounts.Values.ToList();
    }

    public bool BlockAccount(string token, string accountNumber, bool block)
    {
        var user = Authorize(token, "Administrator");

        if (!Accounts.TryGetValue(accountNumber, out var acc))
            throw new FaultException($"Account {accountNumber} not found.");

        acc.IsBlocked = block;
        _logger.LogWarning("Admin {User} {Action} account {Account}",
            user.Identity?.Name, block ? "BLOCKED" : "UNBLOCKED", accountNumber);
        return true;
    }

    public List<Transaction> GetMyTransactions(string token, DateTime from, DateTime to)
    {
        var user = Authorize(token);
        var accountNum = user.FindFirst("AccountNumber")?.Value ?? "";

        _logger.LogInformation("User {User} requested transactions {From:d} - {To:d}",
            user.Identity?.Name, from, to);

        return Transactions
            .Where(t => (t.FromAccount == accountNum || t.ToAccount == accountNum)
                        && t.Timestamp >= from && t.Timestamp <= to)
            .ToList();
    }

    public AuthToken Authenticate(string username, string password)
    {
        _logger.LogInformation("Authentication attempt for user {User}", username);

        // Mock-пользователи для демонстрации
        var (roles, accountNum) = username.ToLower() switch
        {
            "client" when password == "pass123" => (new[] { "Client" }, "ACC001"),
            "operator" when password == "pass123" => (new[] { "Operator" }, "ACC002"),
            "admin" when password == "pass123" => (new[] { "Administrator" }, "ACC003"),
            _ => throw new FaultException("Invalid credentials.")
        };

        var jwt = _jwt.GenerateToken(username, Guid.NewGuid().ToString(), roles, accountNum);

        _logger.LogInformation("User {User} authenticated with roles [{Roles}]", username, string.Join(", ", roles));

        return new AuthToken
        {
            Token = jwt,
            Expires = DateTime.UtcNow.AddHours(2),
            Roles = roles
        };
    }

    public AuthToken RefreshToken(string token)
    {
        var user = Authorize(token);
        var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();
        var accountNum = user.FindFirst("AccountNumber")?.Value ?? "";

        var newJwt = _jwt.GenerateToken(
            user.Identity?.Name ?? "unknown",
            user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "",
            roles, accountNum);

        _logger.LogInformation("Token refreshed for user {User}", user.Identity?.Name);

        return new AuthToken
        {
            Token = newJwt,
            Expires = DateTime.UtcNow.AddHours(2),
            Roles = roles
        };
    }

    private ClaimsPrincipal Authorize(string token, params string[] requiredRoles)
    {
        var principal = _jwt.ValidateToken(token)
            ?? throw new FaultException("Authentication failed. Invalid or expired token.");

        if (requiredRoles.Length > 0)
        {
            var userRoles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value);
            if (!requiredRoles.Any(r => userRoles.Contains(r)))
                throw new FaultException($"Access denied. Required roles: {string.Join(", ", requiredRoles)}");
        }

        return principal;
    }
}
