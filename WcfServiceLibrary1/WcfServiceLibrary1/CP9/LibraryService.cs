using System.Collections.Generic;
using System.Linq;

namespace WcfServiceLibrary1
{
    public class LibraryService : ILibraryService
    {
        // Статическая коллекция книг (общая для всего сервиса)
        private static readonly List<Book> books = new List<Book>();

        public List<Book> GetBooks()
        {
            return books;
        }

        public Book GetBookById(int bookId)
        {
            return books.FirstOrDefault(b => b.BookId == bookId);
        }

        public void AddBook(Book book)
        {
            books.Add(book);
        }
    }
}
