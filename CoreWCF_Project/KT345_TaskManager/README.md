# KT-3 / KT-4 / KT-5. Система управления задачами (Task Manager)

## Описание

Полнофункциональная система управления задачами, построенная по принципам **Clean Architecture**, с CoreWCF-сервером, JWT-аутентификацией, EF Core, FluentValidation, Serilog, юнит-тестами, консольным клиентом и Blazor Server веб-интерфейсом.

Проект объединяет три контрольные точки:

| КТ | Фокус |
|----|-------|
| **КТ-3** | Прототип: контракты, модели, базовый сервис, клиент |
| **КТ-4** | Полный сервер: Clean Architecture, EF Core, Serilog, валидация, тесты |
| **КТ-5** | Клиент: Blazor Server UI, обработка ошибок, интеграция |

## Структура проекта (Clean Architecture)

```
KT345_TaskManager/
│
├── TaskManager.Core/                  # DOMAIN LAYER
│   ├── Contracts/
│   │   └── ITaskManagerService.cs    # [ServiceContract] — 10 операций
│   ├── Models/
│   │   ├── TaskItem.cs               # [DataContract] — задача
│   │   ├── Project.cs                # [DataContract] — проект
│   │   ├── UserInfo.cs               # [DataContract] — пользователь
│   │   ├── WorkloadReport.cs         # [DataContract] — отчёт по нагрузке
│   │   └── AuthToken.cs              # [DataContract] — JWT-токен
│   ├── Enums/
│   │   ├── TaskPriority.cs           # Low / Medium / High / Critical
│   │   └── TaskItemStatus.cs         # New / InProgress / Done / Cancelled
│   └── Interfaces/
│       ├── ITaskRepository.cs        # Интерфейс репозитория задач
│       ├── IProjectRepository.cs     # Интерфейс репозитория проектов
│       └── IUserRepository.cs        # Интерфейс репозитория пользователей
│
├── TaskManager.Application/           # APPLICATION LAYER
│   ├── Services/
│   │   └── TaskAppService.cs         # Бизнес-логика (CRUD, отчёты)
│   └── Validators/
│       └── CreateTaskValidator.cs    # FluentValidation
│
├── TaskManager.Infrastructure/        # INFRASTRUCTURE LAYER
│   ├── Data/
│   │   └── TaskDbContext.cs          # EF Core DbContext + seed data
│   └── Repositories/
│       ├── TaskRepository.cs         # Реализация ITaskRepository
│       ├── ProjectRepository.cs      # Реализация IProjectRepository
│       └── UserRepository.cs         # Реализация IUserRepository
│
├── TaskManager.API/                   # PRESENTATION LAYER (CoreWCF Host)
│   ├── Program.cs                    # DI, Serilog, CoreWCF-эндпоинты
│   ├── appsettings.json              # JWT, Kestrel
│   └── Services/
│       ├── JwtService.cs             # Генерация / валидация JWT
│       └── TaskManagerWcfService.cs  # WCF-фасад над TaskAppService
│
├── TaskManager.ConsoleClient/         # CONSOLE CLIENT
│   └── Program.cs                    # Полный сценарий: auth, CRUD, отчёт
│
├── TaskManager.BlazorClient/          # BLAZOR SERVER CLIENT
│   ├── Program.cs                    # DI + Blazor Server
│   ├── Services/
│   │   └── TaskServiceClient.cs      # WCF-прокси обёртка для Blazor
│   └── Components/Pages/
│       ├── Home.razor                # Страница логина
│       ├── Tasks.razor               # Управление задачами (CRUD)
│       ├── Projects.razor            # Управление проектами
│       └── Workload.razor            # Отчёт по нагрузке
│
└── TaskManager.Tests/                 # UNIT TESTS
    └── TaskAppServiceTests.cs        # 8 тестов (xUnit + Moq)
```

## Архитектура слоёв

```
┌──────────────────────────┐
│  BlazorClient / Console  │  Presentation (клиенты)
├──────────────────────────┤
│    TaskManager.API       │  Presentation (CoreWCF Host)
│  ┌────────────────────┐  │
│  │ TaskManagerWcfService │ — фасад (JWT + FaultException)
│  └────────────────────┘  │
├──────────────────────────┤
│  TaskManager.Application │  Business Logic
│  ┌────────────────────┐  │
│  │   TaskAppService    │ — валидация, бизнес-правила
│  └────────────────────┘  │
├──────────────────────────┤
│ TaskManager.Infrastructure│ Data Access
│  ┌────────────────────┐  │
│  │   EF Core Repos     │ — TaskDbContext (InMemory)
│  └────────────────────┘  │
├──────────────────────────┤
│    TaskManager.Core      │  Domain (центр архитектуры)
│  Models / Interfaces     │  — нулевые зависимости
└──────────────────────────┘
```

**Правило зависимостей:** каждый внешний слой зависит от внутренних, но никогда наоборот. Core не ссылается ни на что.

## Контракт службы (10 операций)

```csharp
[ServiceContract]
public interface ITaskManagerService
{
    // Аутентификация
    AuthToken Authenticate(string username, string password);

    // Задачи (CRUD)
    TaskItem CreateTask(token, title, description, priority, projectId, assigneeId, dueDate);
    TaskItem GetTask(token, taskId);
    List<TaskItem> GetTasksByProject(token, projectId);
    List<TaskItem> GetTasksByAssignee(token, userId);
    bool UpdateTaskStatus(token, taskId, status);
    bool DeleteTask(token, taskId);            // только Admin

    // Проекты
    Project CreateProject(token, name, desc);  // только Admin
    List<Project> GetAllProjects(token);

    // Отчёты
    WorkloadReport GetUserWorkload(token, userId);
}
```

## Ролевая модель

| Операция | Developer | Admin |
|----------|-----------|-------|
| Authenticate | + | + |
| CreateTask | + | + |
| GetTask / GetTasks* | + | + |
| UpdateTaskStatus | + | + |
| GetAllProjects | + | + |
| GetUserWorkload | + | + |
| **DeleteTask** | | + |
| **CreateProject** | | + |

## Тестовые пользователи

| Логин | Пароль | Роль | ID |
|-------|--------|------|----|
| `admin` | `pass123` | Admin | 1 |
| `dev1` | `pass123` | Developer | 2 |
| `dev2` | `pass123` | Developer | 3 |

## Валидация (FluentValidation)

`CreateTaskValidator` проверяет:
- `Title` — обязателен, не более 200 символов
- `ProjectId` — положительное число
- `AssigneeId` — положительное число

При невалидных данных сервис возвращает `FaultException` с конкатенацией ошибок:
```
Title is required.; ProjectId must be positive.
```

## Логирование (Serilog)

- Вывод в **консоль** и **файл** (`logs/taskmanager-YYYYMMDD.log`)
- Structured logging с параметрами:
  ```
  [INF] User dev1 creating task 'Implement login' in project 1
  [WRN] Admin admin deleting task 4
  ```

## Юнит-тесты

8 тестов покрывают `TaskAppService` (Application layer):

| Тест | Проверяет |
|------|-----------|
| `CreateTask_ValidData_ReturnsTask` | Создание задачи с корректными данными |
| `CreateTask_EmptyTitle_ThrowsArgException` | Валидация пустого заголовка |
| `CreateTask_InvalidProjectId_ThrowsArgException` | Несуществующий проект |
| `UpdateTaskStatus_ExistingTask_ReturnsTrue` | Обновление статуса |
| `UpdateTaskStatus_NonExistent_ThrowsArgException` | Обновление несуществующей задачи |
| `GetUserWorkload_ReturnsCorrectReport` | Расчёт отчёта (total, done, overdue) |
| `CreateProject_EmptyName_ThrowsArgException` | Валидация имени проекта |
| `DeleteTask_CallsRepository` | Вызов метода Delete в репозитории |

Фреймворк: **xUnit** + **Moq** (моки репозиториев).

## Blazor Server UI (КТ-5)

4 страницы:

| Страница | URL | Функционал |
|----------|-----|------------|
| **Home** | `/` | Форма логина, отображение роли |
| **Tasks** | `/tasks` | Создание задач, фильтрация по проекту, смена статуса, удаление (Admin) |
| **Projects** | `/projects` | Список проектов, создание (Admin) |
| **Workload** | `/workload` | Отчёт по нагрузке пользователя |

- Авторизация: если пользователь не залогинен — перенаправление на логин.
- Ошибки WCF (`FaultException`) отображаются пользователю в `alert-danger`.
- Blazor Server использует `TaskServiceClient` (scoped DI) — обёртку над `ChannelFactory<ITaskManagerService>`.

## Стек технологий

| Компонент | Технология |
|-----------|-----------|
| Framework | .NET 8.0 |
| WCF | CoreWCF 1.8 |
| ORM | Entity Framework Core 8.0 (InMemory) |
| Валидация | FluentValidation 11.9 |
| Логирование | Serilog (Console + File) |
| Аутентификация | JWT (System.IdentityModel.Tokens.Jwt 8.x) |
| UI | Blazor Server (Interactive) |
| Тесты | xUnit 2.5 + Moq 4.20 |
| Клиент (SOAP) | System.ServiceModel.Http 6.2 |

## Как запустить

### 1. Запуск сервера

```bash
cd KT345_TaskManager
dotnet run --project TaskManager.API
```

Вывод:
```
[INF] Now listening on: http://localhost:5020
```

### 2. Запуск консольного клиента (в отдельном терминале)

```bash
dotnet run --project TaskManager.ConsoleClient
```

Ожидаемый вывод:
```
=== TASK MANAGER CLIENT ===

--- Authentication ---
Admin authenticated. Role=Admin, Expires=20:30:00
Dev1 authenticated. Role=Developer

--- Projects ---
  Project #1: Main Project - Core product development
  Project #2: Mobile App - Mobile application

--- Create Project (Admin) ---
Created: #3 Backend API
[Expected] Access denied. Required roles: Admin

--- Create Task ---
Created: Task #4 'Implement login' [High] -> New

--- Tasks in Project 1 ---
  #1: Setup CI/CD [High] Status=InProgress Assignee=2
  #2: Write unit tests [Medium] Status=New Assignee=3
  #4: Implement login [High] Status=New Assignee=2

--- Update Task Status ---
Task #4: Implement login -> InProgress

--- Workload Report ---
User: Developer One
  Total: 3, Done: 0, InProgress: 2, Overdue: 0

--- Delete Task (Admin) ---
Deleted task #4: True
```

### 3. Запуск Blazor-клиента (в отдельном терминале)

```bash
dotnet run --project TaskManager.BlazorClient
```

Откройте в браузере: `http://localhost:5173` (порт может отличаться, см. вывод).

### 4. Запуск тестов

```bash
dotnet test TaskManager.Tests
```

Результат:
```
Всего тестов: 8
     Пройдено: 8
```

## Эндпоинты

| Тип | Адрес | Описание |
|-----|-------|----------|
| SOAP | `http://localhost:5020/TaskService/basic` | CoreWCF BasicHttpBinding |
| WSDL | `http://localhost:5020/TaskService/basic?wsdl` | Метаданные |
| Health | `GET http://localhost:5020/health` | Статус сервиса |
| Blazor | `http://localhost:5173` | Веб-интерфейс |
