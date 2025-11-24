📌 1. Цель работы

Создать WCF-службу для управления библиотекой книг, включающую:

DataContract-классы для описания книги.

Синхронный RPC-контракт ILibraryService.

Асинхронный RPC-контракт IAsyncLibraryService.

Реализацию служб:

LibraryService (sync)

AsyncLibraryService (async)

Клиентское приложение, вызывающее методы обеих служб.

Работа показывает использование RPC-подхода в WCF и демонстрирует как синхронные, так и Task-based асинхронные операции.

📌 2. Архитектура решения

Решение состоит из трёх ключевых компонентов:

1. WcfServiceLibrary1 (библиотека WCF)

Содержит:

✔ DataContract-класс
[DataContract]
public class Book
{
    [DataMember] public int BookId { get; set; }
    [DataMember] public string Title { get; set; }
    [DataMember] public string Author { get; set; }
    [DataMember] public DateTime PublishedDate { get; set; }
}

✔ Синхронный контракт
[ServiceContract]
public interface ILibraryService
{
    [OperationContract] List<Book> GetBooks();
    [OperationContract] Book GetBookById(int bookId);
    [OperationContract] void AddBook(Book book);
}

✔ Асинхронный контракт
[ServiceContract]
public interface IAsyncLibraryService
{
    [OperationContract] Task<List<Book>> GetBooksAsync();
    [OperationContract] Task<Book> GetBookByIdAsync(int bookId);
    [OperationContract] Task AddBookAsync(Book book);
}

✔ Синхронная реализация
public class LibraryService : ILibraryService
{
    private static readonly List<Book> books = new();

    public List<Book> GetBooks() => books;
    public Book GetBookById(int id) => books.FirstOrDefault(b => b.BookId == id);
    public void AddBook(Book book) => books.Add(book);
}

✔ Асинхронная реализация
public class AsyncLibraryService : IAsyncLibraryService
{
    private static readonly List<Book> books = new();

    public async Task<List<Book>> GetBooksAsync()
    {
        await Task.Delay(100);
        return books;
    }

    public async Task<Book> GetBookByIdAsync(int id)
    {
        await Task.Delay(100);
        return books.FirstOrDefault(b => b.BookId == id);
    }

    public async Task AddBookAsync(Book book)
    {
        await Task.Delay(100);
        books.Add(book);
    }
}

2. WcfServiceHost (self-host)

Службы поднимаются на адресах:

http://localhost:8000/LibraryService
http://localhost:8000/AsyncLibraryService


Endpoint'ы:

libraryHost.AddServiceEndpoint(
    typeof(ILibraryService),
    new BasicHttpBinding(),
    "");

asyncLibraryHost.AddServiceEndpoint(
    typeof(IAsyncLibraryService),
    new BasicHttpBinding(),
    "");


Сервисы запускаются вместе со всеми остальными КТ2–КТ8.

3. LibraryClient9 (консольный клиент)

Клиент выполняет:

синхронный вызов добавления книги;

получение всех книг;

поиск книги по ID;

асинхронный вызов тех же методов.

✔ Синхронная часть
var factory = new ChannelFactory<ILibraryService>(
    new BasicHttpBinding(),
    new EndpointAddress("http://localhost:8000/LibraryService"));

var client = factory.CreateChannel();

client.AddBook(new Book
{
    BookId = 1,
    Title = "C# Programming",
    Author = "John Doe",
    PublishedDate = DateTime.Now
});

var books = client.GetBooks();
var book1 = client.GetBookById(1);

✔ Асинхронная часть
var factory = new ChannelFactory<IAsyncLibraryService>(
    new BasicHttpBinding(),
    new EndpointAddress("http://localhost:8000/AsyncLibraryService"));

var client = factory.CreateChannel();

await client.AddBookAsync(new Book { ... });
var allBooks = await client.GetBooksAsync();
var oneBook = await client.GetBookByIdAsync(2);

📌 3. Демонстрация работы

После запуска клиента:

✔ Синхронный вызов:
Book ID: 1, Title: C# Programming, Author: John Doe, Published Date: ...
Single Book - ID: 1, Title: C# Programming, Author: John Doe

✔ Асинхронный вызов:
Book ID: 2, Title: ASP.NET Core, Author: Jane Doe, Published Date: ...
Single Book - ID: 2, Title: ASP.NET Core, Author: Jane Doe

📌 Итог

RPC-взаимодействие: клиент вызывает методы сервиса так, будто они локальные.

Использование ChannelFactory<T> как RPC-прокси.

DataContract-сериализация объекта Book.

Синхронный и асинхронный контракты: отличие и необходимость.

Асинхронность реализована через Task<T> (.NET TAP-model).

Self-host работает одновременно с другими КТ (2–9).
