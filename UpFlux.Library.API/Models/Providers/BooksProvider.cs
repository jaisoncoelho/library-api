using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UpFlux.Library.API.Db;
using UpFlux.Library.API.Interfaces;

namespace UpFlux.Library.API.Models.Providers
{
    public class BooksProvider : IBooksProvider
    {
        private readonly LibraryDbContext dbContext;
        private readonly ILogger<BooksProvider> logger;
        private readonly IMapper mapper;

        public BooksProvider(LibraryDbContext dbContext, ILogger<BooksProvider> logger,
            IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.Book> Books, string ErrorMessage)> GetBooksAsync(string title, string author)
        {
            try
            {
                var query = dbContext.Books.AsQueryable();
                if (title != null)
                    query = query.Where(b => EF.Functions.Like(b.Title, $"%{title}%"));
                if (author != null)
                    query = query.Where(b => EF.Functions.Like(b.Author, $"%{author}%"));

                var books = await query.Include(b => b.Loans)
                    .Select(b => new { book =  new Db.Book()
                    {
                        Id = b.Id,
                        Title = b.Title,
                        Author = b.Author,
                        IsAvailable = b.Loans.All(b => b.Returned.HasValue)
                    }, count = b.Loans.Count()})
                    .OrderByDescending(b => b.count).Select(b => b.book).ToListAsync();

                if(books != null && books.Any())
                {
                    var result = mapper.Map<IEnumerable<Db.Book>, IEnumerable<Models.Book>>(books);
                    return (true, result, null);
                }
                return (false, null, "Not found");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, Models.Book Book, string ErrorMessage)> GetBookAsync(int id)
        {
            try
            {
                var book = await dbContext.Books.Include(b => b.Loans).Select(b => new Db.Book()
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    IsAvailable = b.Loans.All(b => b.Returned.HasValue)
                }).FirstOrDefaultAsync(b => b.Id == id);
                if(book != null)
                {
                    var result = mapper.Map<Db.Book, Models.Book>(book);
                    return (true, result, null);
                }
                return (false, null, "Not found");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, Models.Book Book, string ErrorMessage)> CreateBookAsync(Models.Book book)
        {
            try
            {
                var entity = mapper.Map<Models.Book, Db.Book>(book);
                await dbContext.AddAsync(entity);
                await dbContext.SaveChangesAsync();

                var result = mapper.Map<Db.Book, Models.Book>(entity);
                return (true, result, null);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, Models.Book Book, string ErrorMessage)> UpdateBookAsync(Models.Book book)
        {
            try
            {
                if (!(await dbContext.Books.AsNoTracking().AnyAsync(b => b.Id == book.Id)))
                    return (false, null, "Not found");

                var entity = mapper.Map<Models.Book, Db.Book>(book);
                dbContext.Update(entity);
                await dbContext.SaveChangesAsync();

                var result = mapper.Map<Db.Book, Models.Book>(entity);
                return (true, result, null);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, string ErrorMessage)> RemoveBookAsync(int id)
        {
            try
            {
                var book = await dbContext.Books.Include(b => b.Loans).FirstOrDefaultAsync(b => b.Id == id);
                if(book != null)
                {
                    dbContext.Remove(book);
                    dbContext.RemoveRange(book.Loans);
                    await dbContext.SaveChangesAsync();

                    return (true, null);
                }
                return (false, "Not found");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return (false, ex.Message);
            }
        }
    }
}
