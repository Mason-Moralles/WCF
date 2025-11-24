using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace WcfServiceLibrary1
{
    [ServiceContract]
    public interface IAsyncLibraryService
    {
        [OperationContract]
        Task<List<Book>> GetBooksAsync();

        [OperationContract]
        Task<Book> GetBookByIdAsync(int bookId);

        [OperationContract]
        Task AddBookAsync(Book book);
    }
}
