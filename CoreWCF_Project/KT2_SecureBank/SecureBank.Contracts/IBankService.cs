using System.ServiceModel;

namespace SecureBank.Contracts;

[ServiceContract]
public interface IBankService
{
    [OperationContract]
    string GetServiceInfo();

    [OperationContract]
    BankAccount GetMyAccount(string token);

    [OperationContract]
    List<BankAccount> GetAllAccounts(string token);

    [OperationContract]
    bool BlockAccount(string token, string accountNumber, bool block);

    [OperationContract]
    List<Transaction> GetMyTransactions(string token, DateTime from, DateTime to);

    [OperationContract]
    AuthToken Authenticate(string username, string password);

    [OperationContract]
    AuthToken RefreshToken(string token);
}
