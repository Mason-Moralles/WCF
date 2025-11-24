using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WcfServiceLibrary1
{
    public class AsyncLibraryService : IAsyncLibraryService
    {
        private static readonly List<Book> books = new List<Book>();

        public async Task<List<Book>> GetBooksAsync()
        {
            await Task.Delay(100); // имитация задержки
            return books;
        }

        public async Task<Book> GetBookByIdAsync(int bookId)
        {
            await Task.Delay(100); // имитация задержки
            return books.FirstOrDefault(b => b.BookId == bookId);
        }

        public async Task AddBookAsync(Book book)
        {
            await Task.Delay(100); // имитация задержки
            books.Add(book);
        }
    }
}
