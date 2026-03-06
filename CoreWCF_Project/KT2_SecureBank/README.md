# KT-2. Реализация защищённого сервиса и клиента

## Описание

Защищённый банковский CoreWCF-сервис (**SecureBankService**) с аутентификацией на основе **JWT-токенов**, ролевой авторизацией (3 роли), аудитом операций и обработкой ошибок через `FaultException`. Клиентское приложение последовательно демонстрирует доступ от имени каждой из трёх ролей.

## Структура проекта

```
KT2_SecureBank/
├── SecureBank.Contracts/              # Контракты (shared между сервером и клиентом)
│   ├── IBankService.cs               # [ServiceContract] — 7 операций
│   ├── BankAccount.cs                # [DataContract] — банковский счёт
│   ├── Transaction.cs                # [DataContract] — транзакция
│   └── AuthToken.cs                  # [DataContract] — JWT-токен + роли
│
├── SecureBank.Service/                # CoreWCF-хост с JWT
│   ├── Program.cs                    # Настройка Identity + CoreWCF + REST auth endpoint
│   ├── appsettings.json              # JWT secret, Kestrel-конфигурация
│   ├── Models/
│   │   └── ApplicationUser.cs        # IdentityUser с AccountNumber
│   ├── Data/
│   │   └── AppDbContext.cs           # IdentityDbContext (EF Core InMemory)
│   └── Services/
│       ├── JwtService.cs             # Генерация и валидация JWT-токенов
│       └── BankService.cs            # Реализация IBankService с авторизацией
│
└── SecureBank.Client/                 # Консольный клиент
    └── Program.cs                    # Демонстрация 3 ролей (Client/Operator/Admin)
```

## Контракт службы

```csharp
[ServiceContract]
public interface IBankService
{
    [OperationContract] string GetServiceInfo();
    [OperationContract] BankAccount GetMyAccount(string token);
    [OperationContract] List<BankAccount> GetAllAccounts(string token);
    [OperationContract] bool BlockAccount(string token, string accountNumber, bool block);
    [OperationContract] List<Transaction> GetMyTransactions(string token, DateTime from, DateTime to);
    [OperationContract] AuthToken Authenticate(string username, string password);
    [OperationContract] AuthToken RefreshToken(string token);
}
```

## Матрица доступа (RBAC)

| Операция | Public | Client | Operator | Administrator |
|----------|--------|--------|----------|---------------|
| `GetServiceInfo` | + | + | + | + |
| `Authenticate` | + | + | + | + |
| `GetMyAccount` | | + | + | + |
| `GetMyTransactions` | | + | + | + |
| `RefreshToken` | | + | + | + |
| `GetAllAccounts` | | | + | + |
| `BlockAccount` | | | | + |

При попытке вызова метода без достаточных прав сервис возвращает `FaultException` с сообщением:
```
Access denied. Required roles: Administrator
```

## Тестовые пользователи

| Логин | Пароль | Роль | Счёт | Баланс |
|-------|--------|------|------|--------|
| `client` | `pass123` | Client | ACC001 | 10 000 USD |
| `operator` | `pass123` | Operator | ACC002 | 5 000 EUR |
| `admin` | `pass123` | Administrator | ACC003 | 15 000 GBP |

## Архитектура безопасности

### JWT-аутентификация

1. Клиент вызывает `Authenticate(username, password)` через SOAP.
2. Сервис валидирует учётные данные и генерирует JWT-токен с claims:
   - `ClaimTypes.Name` — имя пользователя
   - `ClaimTypes.NameIdentifier` — уникальный ID
   - `ClaimTypes.Role` — роль (Client / Operator / Administrator)
   - `AccountNumber` — привязанный номер счёта
3. Токен подписывается HMAC-SHA256 (секрет в `appsettings.json`).
4. Все последующие вызовы принимают `token` как параметр.
5. Сервис валидирует JWT и извлекает claims для авторизации.

### Конфигурация JWT

```json
{
  "JwtSettings": {
    "Secret": "ThisIsASuperSecretKeyForJWTTokenMinimum32Chars!",
    "Issuer": "SecureBankService",
    "Audience": "BankClient"
  }
}
```

### ASP.NET Core Identity

- `ApplicationUser` наследует `IdentityUser`, добавляя поля `FullName` и `AccountNumber`.
- `AppDbContext` — `IdentityDbContext<ApplicationUser>` на EF Core InMemory.
- При старте приложения выполняется seed (создание ролей и пользователей).

### Аудит операций

Все операции логируются через `ILogger<BankService>`:

```
info: BankService  User admin accessed all accounts
warn: BankService  Admin admin BLOCKED account ACC001
```

- `LogInformation` — штатные операции (чтение, аутентификация).
- `LogWarning` — административные действия (блокировка/разблокировка).

### Обработка ошибок

Все ошибки бизнес-логики возвращаются клиенту через `FaultException`:

```csharp
throw new FaultException("Authentication failed. Invalid or expired token.");
throw new FaultException($"Access denied. Required roles: {string.Join(", ", requiredRoles)}");
```

## Эндпоинты

| Тип | Адрес | Описание |
|-----|-------|----------|
| SOAP (HTTP) | `http://localhost:5010/BankService/basic` | CoreWCF BasicHttpBinding |
| REST Auth | `POST http://localhost:5010/api/auth/login` | Получение JWT (JSON) |
| Health | `GET http://localhost:5010/health` | Статус сервиса |
| WSDL | `http://localhost:5010/BankService/basic?wsdl` | Метаданные |

## Стек технологий

| Компонент | Технология |
|-----------|-----------|
| Framework | .NET 8.0 |
| WCF | CoreWCF 1.8 (Http) |
| Аутентификация | JWT (System.IdentityModel.Tokens.Jwt 8.x) |
| Identity | ASP.NET Core Identity + EF Core InMemory |
| Логирование | Microsoft.Extensions.Logging |
| Клиент | System.ServiceModel.Http 6.2 |

## Как запустить

### 1. Запуск сервиса

```bash
cd KT2_SecureBank
dotnet run --project SecureBank.Service
```

Вывод:
```
info: Microsoft.Hosting.Lifetime      Now listening on: http://localhost:5010
```

### 2. Запуск клиента (в отдельном терминале)

```bash
dotnet run --project SecureBank.Client
```

### Ожидаемый вывод клиента

```
=== SECURE BANK CLIENT ===

[Public] Secure Bank Service v1.0 — JWT authentication required for most operations.

--- CLIENT ROLE ---
Authenticated: roles=[Client], expires=20:15:30
My account: ACC001, Balance=10000 USD
Transactions: 2 found
  #1: 100 Transfer (2026-03-03)
  #3: 250 Payment (2026-02-27)
[Expected] Access denied: Access denied. Required roles: Operator, Administrator

--- OPERATOR ROLE ---
Authenticated: roles=[Operator]
All accounts: 3
  ACC001: 10000 USD
  ACC002: 5000 EUR
  ACC003: 15000 GBP [BLOCKED]
[Expected] Access denied: Access denied. Required roles: Administrator

--- ADMIN ROLE ---
Authenticated: roles=[Administrator]
Block ACC001: True
  ACC001: 10000 USD [BLOCKED]
  ACC002: 5000 EUR
  ACC003: 15000 GBP [BLOCKED]
ACC001 unblocked.

--- TOKEN REFRESH ---
New token expires: 20:15:31, roles=[Administrator]
```

> Строки `[Expected] Access denied` подтверждают, что ролевая авторизация работает корректно: Client не может вызвать `GetAllAccounts`, Operator не может вызвать `BlockAccount`.
