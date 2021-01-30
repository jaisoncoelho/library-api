using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UpFlux.Library.API.Db;
using UpFlux.Library.API.Models.Providers;
using UpFlux.Library.API.Profiles;
using System.Linq;

namespace UpFlux.Library.API.Tests
{
    [TestClass]
    public class BookTests
    {
        private readonly LibraryDbContext dbContext;
        private readonly ILogger<BooksProvider> logger;
        private readonly IMapper mapper;

        public BookTests()
        {
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(nameof(BookTests))
                .Options;
            dbContext = new LibraryDbContext(options);
            
            var loggerMock = new Mock<ILogger<BooksProvider>>();
            logger = loggerMock.Object;

            var libraryProfile = new LibraryProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(libraryProfile));
            mapper = new Mapper(configuration);

            CreateBooks();
        }

        private void CreateBooks()
        {
            foreach (var entity in dbContext.Books)
                dbContext.Books.Remove(entity);
            dbContext.SaveChanges();

            dbContext.Add(new Db.Book { Id = 100, Title = "A Alma do Mundo", Author = "Roger Scruton" });
            dbContext.Add(new Db.Book { Id = 101, Title = "Beleza", Author = "Roger Scruton", Loans = new List<Db.Loan>()
            {
                new Loan() { User = "Maria", Borrowed = DateTime.Now }
            }
            });
            dbContext.Add(new Db.Book { Id = 102, Title = "O Homem Eterno", Author = "G. K. Chesterton", Loans = new List<Db.Loan>()
            {
                new Loan() { User = "João", Borrowed = DateTime.Now.AddDays(-2), Returned = DateTime.Now.AddDays(-1) },
                new Loan() { User = "Jaison", Borrowed = DateTime.Now }
            }
            });

            dbContext.SaveChanges();
            dbContext.ChangeTracker.Clear();
        }

        [TestMethod]       
        public async Task GetBooksReturnsAllBooks()
        {            
            var booksProvider = new BooksProvider(dbContext, logger, mapper);

            var result = await booksProvider.GetBooksAsync(null, null);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Books);
            Assert.IsTrue(result.Books.Count() == 3);
            Assert.IsNull(result.ErrorMessage);
        }

        public async Task GetBooksReturnsAllBooksOrdenedByBorrowingsNumber()
        {
            var booksProvider = new BooksProvider(dbContext, logger, mapper);

            var result = await booksProvider.GetBooksAsync(null, null);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Books);
            Assert.IsTrue(result.Books.Count() == 3);
            Assert.IsTrue(result.Books.ElementAt(0).Id == 102);
            Assert.IsTrue(result.Books.ElementAt(1).Id == 101);
            Assert.IsNull(result.ErrorMessage);
        }
    
        [TestMethod]
        public async Task GetBooksReturnsSpecificBooks()
        {                     
            var booksProvider = new BooksProvider(dbContext, logger, mapper);

            var result = await booksProvider.GetBooksAsync("Alma", "Scruton");

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Books);
            Assert.IsTrue(result.Books.Count() == 1);
            Assert.IsTrue(result.Books.Any(b => b.Id == 100));
            Assert.IsNull(result.ErrorMessage);
        }
    
        [TestMethod]
        public async Task GetBooksReturnsError()
        {            
            var booksProvider = new BooksProvider(dbContext, logger, mapper);            

            var result = await booksProvider.GetBooksAsync(null, Guid.NewGuid().ToString());

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNull(result.Books);            
            Assert.IsNotNull(result.ErrorMessage);
        }

        [TestMethod]
        public async Task GetBookReturnsBook()
        {                        
            var booksProvider = new BooksProvider(dbContext, logger, mapper);

            var result = await booksProvider.GetBookAsync(100);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Book);
            Assert.IsTrue(result.Book.Id == 100);
            Assert.IsNull(result.ErrorMessage);
        }

        [TestMethod]
        public async Task GetCurrentlyBorrowedBookReturnsAvailableFlagEqualsToFalse()
        {
            var booksProvider = new BooksProvider(dbContext, logger, mapper);

            var result = await booksProvider.GetBookAsync(102);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Book);
            Assert.IsTrue(result.Book.Id == 102);
            Assert.IsTrue(result.Book.IsAvailable == false);
            Assert.IsNull(result.ErrorMessage);
        }

        [TestMethod]
        public async Task GetNotBorrowedBookReturnsAvailableFlagEqualsToTrue()
        {
            var booksProvider = new BooksProvider(dbContext, logger, mapper);

            var result = await booksProvider.GetBookAsync(100);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Book);
            Assert.IsTrue(result.Book.Id == 100);
            Assert.IsTrue(result.Book.IsAvailable == true);
            Assert.IsNull(result.ErrorMessage);
        }

        [TestMethod]
        public async Task GetBookReturnsError()
        {            
            var booksProvider = new BooksProvider(dbContext, logger, mapper);

            var result = await booksProvider.GetBookAsync(-1);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNull(result.Book);            
            Assert.IsNotNull(result.ErrorMessage);
        }

        [TestMethod]
        public async Task CreateBook()
        {            
            var booksProvider = new BooksProvider(dbContext, logger, mapper);

            var result = await booksProvider.CreateBookAsync(new Models.Book()
            {
                Title = "Segundo Tratado Sobre o Governo",
                Author = "John Locke"
            });

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Book);
            Assert.IsNull(result.ErrorMessage);
        }

        [TestMethod]
        public async Task UpdateBook_Success()
        {            
            var booksProvider = new BooksProvider(dbContext, logger, mapper);

            var result = await booksProvider.UpdateBookAsync(new Models.Book()
            {
                Id = 102,
                Title = "O Que Há De Errado Com O Mundo",
                Author = "G. K. Chesterton"
            });

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Book);
            Assert.IsTrue(result.Book.Title == "O Que Há De Errado Com O Mundo");
            Assert.IsNull(result.ErrorMessage);
        }

        [TestMethod]
        public async Task UpdateBook_Fail()
        {            
            var booksProvider = new BooksProvider(dbContext, logger, mapper);

            var result = await booksProvider.UpdateBookAsync(new Models.Book()
            {
                Id = 105,
                Title = "O Que Há De Errado Com O Mundo",
                Author = "G. K. Chesterton"
            });

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNull(result.Book);            
            Assert.IsNotNull(result.ErrorMessage);
        }

        [TestMethod]
        public async Task RemoveBook_Success()
        {            
            var booksProvider = new BooksProvider(dbContext, logger, mapper);

            var result = await booksProvider.RemoveBookAsync(100);

            Assert.IsTrue(result.IsSuccess);        
            Assert.IsNull(result.ErrorMessage);
            Assert.IsTrue(!(await dbContext.Books.AnyAsync(b => b.Id == 100)));
        }

        [TestMethod]
        public async Task RemoveBook_Fail()
        {            
            var booksProvider = new BooksProvider(dbContext, logger, mapper);

            var result = await booksProvider.RemoveBookAsync(105);

            Assert.IsFalse(result.IsSuccess);            
            Assert.IsNotNull(result.ErrorMessage);
        }
    }
}
