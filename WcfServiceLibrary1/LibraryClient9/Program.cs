using System;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfServiceLibrary1;

namespace LibraryClient9
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== КТ9: Library RPC Client ===");

            RunSyncClient();
            Console.WriteLine();
            RunAsyncClient().Wait();

            Console.WriteLine();
            Console.WriteLine("Готово. Нажмите Enter для выхода...");
            Console.ReadLine();
        }

        // Синхронный клиент
        private static void RunSyncClient()
        {
            Console.WriteLine("\nSync Client:\n");

            var factory = new ChannelFactory<ILibraryService>(
                new BasicHttpBinding(),
                new EndpointAddress("http://localhost:8000/LibraryService"));

            var client = factory.CreateChannel();

            try
            {
                // Добавляем книгу
                client.AddBook(new Book
                {
                    BookId = 1,
                    Title = "C# Programming",
                    Author = "John Doe",
                    PublishedDate = DateTime.Now
                });

                // Получаем список всех книг
                var books = client.GetBooks();
                foreach (var book in books)
                {
                    Console.WriteLine(
                        $"Book ID: {book.BookId}, Title: {book.Title}, Author: {book.Author}, Published Date: {book.PublishedDate}");
                }

                // Получаем книгу по ID
                var singleBook = client.GetBookById(1);
                Console.WriteLine(
                    $"Single Book - ID: {singleBook.BookId}, Title: {singleBook.Title}, Author: {singleBook.Author}");
            }
            finally
            {
                try { ((IClientChannel)client).Close(); } catch { ((IClientChannel)client).Abort(); }
                try { factory.Close(); } catch { factory.Abort(); }
            }
        }

        // Асинхронный клиент
        private static async Task RunAsyncClient()
        {
            Console.WriteLine("\nAsync Client:\n");

            var asyncFactory = new ChannelFactory<IAsyncLibraryService>(
                new BasicHttpBinding(),
                new EndpointAddress("http://localhost:8000/AsyncLibraryService"));

            var asyncClient = asyncFactory.CreateChannel();

            try
            {
                await asyncClient.AddBookAsync(new Book
                {
                    BookId = 2,
                    Title = "ASP.NET Core",
                    Author = "Jane Doe",
                    PublishedDate = DateTime.Now
                });

                var asyncBooks = await asyncClient.GetBooksAsync();
                foreach (var book in asyncBooks)
                {
                    Console.WriteLine(
                        $"Book ID: {book.BookId}, Title: {book.Title}, Author: {book.Author}, Published Date: {book.PublishedDate}");
                }

                var asyncSingleBook = await asyncClient.GetBookByIdAsync(2);
                Console.WriteLine(
                    $"Single Book - ID: {asyncSingleBook.BookId}, Title: {asyncSingleBook.Title}, Author: {asyncSingleBook.Author}");
            }
            finally
            {
                try { ((IClientChannel)asyncClient).Close(); } catch { ((IClientChannel)asyncClient).Abort(); }
                try { asyncFactory.Close(); } catch { asyncFactory.Abort(); }
            }
        }
    }
}
