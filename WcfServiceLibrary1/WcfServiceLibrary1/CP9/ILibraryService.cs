using System.Collections.Generic;
using System.ServiceModel;

namespace WcfServiceLibrary1
{
    [ServiceContract]
    public interface ILibraryService
    {
        [OperationContract]
        List<Book> GetBooks();

        [OperationContract]
        Book GetBookById(int bookId);

        [OperationContract]
        void AddBook(Book book);
    }
}
