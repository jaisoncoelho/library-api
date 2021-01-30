using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UpFlux.Library.API.Models;

namespace UpFlux.Library.API.Interfaces
{
    public interface IBooksProvider
    {
        Task<(bool IsSuccess, IEnumerable<Models.Book> Books, string ErrorMessage)> GetBooksAsync(string title, string author);
        Task<(bool IsSuccess, Models.Book Book, string ErrorMessage)> GetBookAsync(int id);
        Task<(bool IsSuccess, Models.Book Book, string ErrorMessage)> CreateBookAsync(Models.Book book);
        Task<(bool IsSuccess, Models.Book Book, string ErrorMessage)> UpdateBookAsync(Models.Book book);
        Task<(bool IsSuccess, string ErrorMessage)> RemoveBookAsync(int id);
    }
}
