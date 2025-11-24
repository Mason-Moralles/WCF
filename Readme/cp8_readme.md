RPC-служба управления заказами в WCF (синхронные и асинхронные вызовы)

Данная работа выполнена в рамках КТ8 и демонстрирует реализацию RPC-подхода в WCF с использованием синхронных и асинхронных контрактов.
Проект состоит из WCF-службы управления заказами и клиентского приложения, которое вызывает методы сервиса через BasicHttpBinding.

📌 1. Цель работы

Создать WCF-службу, реализующую RPC-вызовы, для управления заказами:

Синхронный контракт: получение, добавление и поиск заказов.

Асинхронный контракт: то же самое, но через Task<>.

DataContract-классы для описания заказов.

Клиент, использующий RPC-подход через ChannelFactory<T>.

Работа демонстрирует особенности RPC в WCF и работу с синхронными/асинхронными методами.

📌 2. Архитектура решения

Решение включает три основных компонента:

1. Библиотека WCF (WcfServiceLibrary1)

Содержит:

DataContract-классы:

Order8

OrderItem8

Контракты:

IOrderService8 (синхронный)

IOrderService8Async (асинхронный)

Реализации:

OrderService8

OrderService8Async

Статический репозиторий OrderStore8 — общая коллекция заказов.

2. Self-host (WcfServiceHost)

Поднимает два сервиса:

http://localhost:8760/OrderService8
http://localhost:8760/OrderService8Async


Оба используют BasicHttpBinding, что соответствует RPC-направлению WCF.

3. Клиент (OrderRpcClient)

Консольное приложение, выполняющее:

Синхронные вызовы (IOrderService8)

Асинхронные вызовы (IOrderService8Async)

Через:

new ChannelFactory<IOrderService8>(binding, endpoint);


что и реализует RPC-подход.

📌 3. DataContract-классы

Используется вложенная структура: заказ + список товарных позиций.

[DataContract]
public class OrderItem8
{
    [DataMember] public int ProductId { get; set; }
    [DataMember] public string ProductName { get; set; }
    [DataMember] public int Quantity { get; set; }
}

[DataContract]
public class Order8
{
    [DataMember] public int OrderId { get; set; }
    [DataMember] public string CustomerName { get; set; }
    [DataMember] public DateTime CreatedAt { get; set; }
    [DataMember] public List<OrderItem8> Items { get; set; } = new();
}

📌 4. Синхронный контракт и реализация
IOrderService8
[ServiceContract]
public interface IOrderService8
{
    [OperationContract] List<Order8> GetOrders();
    [OperationContract] Order8 GetOrderById(int id);
    [OperationContract] void AddOrder(Order8 order);
}

OrderService8
public class OrderService8 : IOrderService8
{
    public List<Order8> GetOrders() => OrderStore8.GetAll();
    public Order8 GetOrderById(int id) => OrderStore8.GetById(id);
    public void AddOrder(Order8 order) => OrderStore8.Add(order);
}

📌 5. Асинхронный контракт и реализация
IOrderService8Async
[ServiceContract]
public interface IOrderService8Async
{
    [OperationContract] Task<List<Order8>> GetOrdersAsync();
    [OperationContract] Task<Order8> GetOrderByIdAsync(int id);
    [OperationContract] Task AddOrderAsync(Order8 order);
}

OrderService8Async
public class OrderService8Async : IOrderService8Async
{
    public Task<List<Order8>> GetOrdersAsync() => Task.FromResult(OrderStore8.GetAll());
    public Task<Order8> GetOrderByIdAsync(int id) => Task.FromResult(OrderStore8.GetById(id));
    public Task AddOrderAsync(Order8 order) { OrderStore8.Add(order); return Task.CompletedTask; }
}

📌 6. Поднятие сервисов в Self-host

В WcfServiceHost оба сервиса поднимаются так:

var orderHost8 = new ServiceHost(
    typeof(OrderService8),
    new Uri("http://localhost:8760/OrderService8"));
orderHost8.AddServiceEndpoint(typeof(IOrderService8), new BasicHttpBinding(), "");

var orderHost8Async = new ServiceHost(
    typeof(OrderService8Async),
    new Uri("http://localhost:8760/OrderService8Async"));
orderHost8Async.AddServiceEndpoint(typeof(IOrderService8Async), new BasicHttpBinding(), "");

📌 7. RPC-клиент (OrderRpcClient)

Клиент использует ChannelFactory<T> — это ключевой признак RPC:

var factory = new ChannelFactory<IOrderService8>(
    new BasicHttpBinding(),
    new EndpointAddress("http://localhost:8760/OrderService8"));

var client = factory.CreateChannel();


Клиент демонстрирует:

получение списка заказов

добавление нового заказа

поиск по ID

асинхронные версии тех же операций

RPC-взаимодействие выглядит как обычный вызов локальных методов:

var result = client.GetOrders();   // sync
var result2 = await client.GetOrdersAsync(); // async

📌 8. Демонстрация работы
Синхронный вызов:
=== Синхронные вызовы IOrderService8 ===
Всего заказов: 2
Добавлен новый заказ для Charlie.
Заказ #1: Alice

Асинхронный вызов:
=== Асинхронные вызовы IOrderService8Async ===
[Async] Всего заказов: 3
[Async] Заказ #2: Bob
[Async] Добавлен заказ для AsyncUser.
[Async] Заказов после добавления: 4

📌 9. Вывод

✔ реализован WCF-сервис управления заказами в RPC-стиле
✔ созданы DataContract-классы заказов
✔ реализованы синхронные и асинхронные контракты
✔ выполнена реализация сервисов
✔ сервисы размещены в self-host
✔ разработан клиент, выполняющий RPC-вызовы
✔ продемонстрирована работа sync/async RPC