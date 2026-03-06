using System.ServiceModel;
using SecureBank.Contracts;

Console.WriteLine("=== SECURE BANK CLIENT ===\n");

var binding = new BasicHttpBinding();
var endpoint = new EndpointAddress("http://localhost:5010/BankService/basic");
var factory = new ChannelFactory<IBankService>(binding, endpoint);
var client = factory.CreateChannel();

try
{
    // 1. Публичный метод (без аутентификации)
    var info = client.GetServiceInfo();
    Console.WriteLine($"[Public] {info}\n");

    // 2. Аутентификация как Client
    Console.WriteLine("--- CLIENT ROLE ---");
    var clientToken = client.Authenticate("client", "pass123");
    Console.WriteLine($"Authenticated: roles=[{string.Join(", ", clientToken.Roles)}], expires={clientToken.Expires:HH:mm:ss}");

    var myAccount = client.GetMyAccount(clientToken.Token);
    Console.WriteLine($"My account: {myAccount.AccountNumber}, Balance={myAccount.Balance} {myAccount.Currency}");

    var transactions = client.GetMyTransactions(clientToken.Token, DateTime.UtcNow.AddDays(-30), DateTime.UtcNow);
    Console.WriteLine($"Transactions: {transactions.Count} found");
    foreach (var t in transactions)
        Console.WriteLine($"  #{t.Id}: {t.Amount} {t.Description} ({t.Timestamp:yyyy-MM-dd})");

    // Client пытается вызвать GetAllAccounts (должен быть отказ)
    try
    {
        client.GetAllAccounts(clientToken.Token);
        Console.WriteLine("ERROR: Client should not access GetAllAccounts!");
    }
    catch (FaultException ex)
    {
        Console.WriteLine($"[Expected] Access denied: {ex.Message}");
    }

    // 3. Аутентификация как Operator
    Console.WriteLine("\n--- OPERATOR ROLE ---");
    var operatorToken = client.Authenticate("operator", "pass123");
    Console.WriteLine($"Authenticated: roles=[{string.Join(", ", operatorToken.Roles)}]");

    var allAccounts = client.GetAllAccounts(operatorToken.Token);
    Console.WriteLine($"All accounts: {allAccounts.Count}");
    foreach (var acc in allAccounts)
        Console.WriteLine($"  {acc.AccountNumber}: {acc.Balance} {acc.Currency} {(acc.IsBlocked ? "[BLOCKED]" : "")}");

    // Operator пытается заблокировать счёт (должен быть отказ)
    try
    {
        client.BlockAccount(operatorToken.Token, "ACC001", true);
        Console.WriteLine("ERROR: Operator should not block accounts!");
    }
    catch (FaultException ex)
    {
        Console.WriteLine($"[Expected] Access denied: {ex.Message}");
    }

    // 4. Аутентификация как Administrator
    Console.WriteLine("\n--- ADMIN ROLE ---");
    var adminToken = client.Authenticate("admin", "pass123");
    Console.WriteLine($"Authenticated: roles=[{string.Join(", ", adminToken.Roles)}]");

    var blocked = client.BlockAccount(adminToken.Token, "ACC001", true);
    Console.WriteLine($"Block ACC001: {blocked}");

    allAccounts = client.GetAllAccounts(adminToken.Token);
    foreach (var acc in allAccounts)
        Console.WriteLine($"  {acc.AccountNumber}: {acc.Balance} {acc.Currency} {(acc.IsBlocked ? "[BLOCKED]" : "")}");

    // Разблокировка
    client.BlockAccount(adminToken.Token, "ACC001", false);
    Console.WriteLine("ACC001 unblocked.");

    // 5. Refresh token
    Console.WriteLine("\n--- TOKEN REFRESH ---");
    var newToken = client.RefreshToken(adminToken.Token);
    Console.WriteLine($"New token expires: {newToken.Expires:HH:mm:ss}, roles=[{string.Join(", ", newToken.Roles)}]");
}
catch (Exception ex)
{
    Console.WriteLine($"\nFATAL ERROR: {ex.Message}");
}
finally
{
    ((IClientChannel)client).Close();
}

Console.WriteLine("\nНажмите любую клавишу для выхода...");
Console.ReadKey();
