# KT-1. Хостинг CoreWCF-службы в ASP.NET Core, настройка транспорта (HTTP, TCP)

## Описание

CoreWCF-служба мониторинга устройств (**DeviceMonitoringService**), размещённая в ASP.NET Core, с одновременной поддержкой двух транспортных протоколов — **HTTP** (`BasicHttpBinding`) и **TCP** (`NetTcpBinding`). Для тестирования каждого типа подключения реализованы два консольных клиента.

## Структура проекта

```
KT1_DeviceMonitoring/
├── DeviceMonitoring.Contracts/        # Контракты данных и службы (shared)
│   ├── DeviceInfo.cs                  # [DataContract] — модель устройства
│   └── IDeviceManager.cs             # [ServiceContract] — 4 операции
│
├── DeviceMonitoring.Service/          # CoreWCF-хост (ASP.NET Core)
│   ├── DeviceService.cs              # Реализация IDeviceManager
│   ├── Program.cs                    # Настройка HTTP + TCP эндпоинтов
│   └── appsettings.json              # Конфигурация портов Kestrel
│
├── DeviceMonitoring.HttpClient/       # Консольный HTTP-клиент
│   └── Program.cs                    # BasicHttpBinding, вызов всех 4 операций
│
└── DeviceMonitoring.TcpClient/        # Консольный TCP-клиент
    └── Program.cs                    # NetTcpBinding, замер производительности
```

## Контракт данных

```csharp
[DataContract]
public class DeviceInfo
{
    [DataMember] public int Id { get; set; }
    [DataMember] public string Name { get; set; }
    [DataMember] public DateTime LastPing { get; set; }
    [DataMember] public bool IsOnline { get; set; }
}
```

## Контракт службы

```csharp
[ServiceContract]
public interface IDeviceManager
{
    [OperationContract] List<DeviceInfo> GetAllDevices();
    [OperationContract] DeviceInfo GetDevice(int id);
    [OperationContract] bool PingDevice(int id);
    [OperationContract] string GetServiceStats();
}
```

| Операция | Описание |
|----------|----------|
| `GetAllDevices()` | Возвращает список всех 10 устройств |
| `GetDevice(id)` | Возвращает устройство по ID |
| `PingDevice(id)` | Обновляет `LastPing` и `IsOnline`, возвращает `true` при успехе |
| `GetServiceStats()` | Возвращает количество вызовов через HTTP и TCP |

## Технические решения

### Двойной транспорт

Сервис одновременно слушает на двух портах:

| Транспорт | Binding | Адрес | Настройка |
|-----------|---------|-------|-----------|
| HTTP | `BasicHttpBinding` | `http://localhost:5000/DeviceService/basic` | Kestrel (`appsettings.json`) |
| TCP | `NetTcpBinding` | `net.tcp://localhost:8090/DeviceService/nettcp` | `builder.WebHost.UseNetTcp(8090)` |

> **Важно:** `net.tcp://` не является HTTP-схемой — Kestrel его **не понимает**.
> TCP-транспорт CoreWCF регистрируется отдельно через `builder.WebHost.UseNetTcp(port)`,
> после чего CoreWCF самостоятельно управляет этим сокетом поверх Kestrel.

```csharp
// Program.cs — правильная настройка TCP
builder.WebHost.UseNetTcp(8090);          // регистрирует TCP-порт

// в UseServiceModel — абсолютный URI для TCP-эндпоинта
serviceBuilder.AddServiceEndpoint<DeviceService, IDeviceManager>(
    new NetTcpBinding(SecurityMode.None),
    new Uri("net.tcp://localhost:8090/DeviceService/nettcp"));
```

### Внедрение зависимостей (DI)

- `DeviceService` зарегистрирован как **Singleton** — единое хранилище данных и счётчиков на протяжении всего жизненного цикла приложения.
- CoreWCF подключается через `AddServiceModelServices()` / `AddServiceModelMetadata()`.

### Подсчёт вызовов по транспорту

Метод `TrackTransport()` определяет текущий транспорт через `OperationContext.Current`:

```csharp
var scheme = context.IncomingMessageProperties?.Via?.Scheme;
if (scheme == "http" || scheme == "https")
    Interlocked.Increment(ref _httpCallCount);
else
    Interlocked.Increment(ref _tcpCallCount);
```

- Потокобезопасный инкремент через `Interlocked.Increment`.
- Результат доступен через метод `GetServiceStats()`.

### Хранилище данных

- `ConcurrentDictionary<int, DeviceInfo>` — потокобезопасный словарь.
- Инициализируется 10 устройствами (`Device_1` .. `Device_10`).

### WSDL-метаданные

Метаданные доступны по адресу:
```
http://localhost:5000/DeviceService/basic?wsdl
```

### Health-check

```
GET http://localhost:5000/health
```

## Стек технологий

| Компонент | Технология |
|-----------|-----------|
| Framework | .NET 8.0 |
| WCF | CoreWCF 1.8 (Http + NetTcp) |
| Хостинг | ASP.NET Core (Kestrel) |
| DI | Microsoft.Extensions.DependencyInjection |
| Клиенты | System.ServiceModel.Http / .NetTcp 6.2 |

## Как запустить

### 1. Запуск сервиса

```bash
cd KT1_DeviceMonitoring
dotnet run --project DeviceMonitoring.Service
```

Вывод в консоли:
```
info: CoreWCF  Mapping CoreWCF branch app for path /DeviceService/basic
info: Microsoft.Hosting.Lifetime      Now listening on: http://0.0.0.0:8090
info: Microsoft.Hosting.Lifetime      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime      Application started. Press Ctrl+C to shut down.
```

> `http://0.0.0.0:8090` в логах — это как Kestrel показывает TCP-порт CoreWCF.
> Клиент подключается по адресу `net.tcp://localhost:8090/...` через `NetTcpBinding`.

### 2. Запуск HTTP-клиента (в отдельном терминале)

```bash
dotnet run --project DeviceMonitoring.HttpClient
```

Ожидаемый вывод:
```
=== HTTP CLIENT (BasicHttpBinding) ===

[Stats] HTTP calls: 1, TCP calls: 0

[All Devices] Total: 10
  ID=1, Name=Device_1, Online=False, LastPing=...
  ID=2, Name=Device_2, Online=True, LastPing=...
  ...

[GetDevice(1)] Device_1, Online=False

[PingDevice(2)] Result: True

[Final Stats] HTTP calls: 5, TCP calls: 0
```

### 3. Запуск TCP-клиента (в отдельном терминале)

```bash
dotnet run --project DeviceMonitoring.TcpClient
```

Ожидаемый вывод:
```
=== TCP CLIENT (NetTcpBinding) ===

[GetDevice(3)] Device_3, Online=False (42 ms)
[PingDevice(3)] Result: True (3 ms)
[Stats] HTTP calls: 5, TCP calls: 3 (2 ms)

=== PERFORMANCE TEST (100 calls GetDevice) ===
100 TCP calls: 156 ms
Average: 1.56 ms per call
```

## Сравнение HTTP vs TCP

| Параметр | HTTP (BasicHttp) | TCP (NetTcp) |
|----------|-----------------|--------------|
| Протокол | HTTP/1.1 + SOAP/XML | Бинарный TCP |
| Формат сообщений | Text/XML | Binary |
| Первый вызов | ~200 ms | ~40 ms |
| Среднее время | ~5-10 ms | ~1-3 ms |
| Использование | Межплатформенная совместимость | Высокая производительность (intranet) |

> TCP-транспорт значительно быстрее за счёт бинарной сериализации и постоянного TCP-соединения.
