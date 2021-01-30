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
    public class LoanTests
    {
        private readonly LibraryDbContext dbContext;
        private readonly ILogger<LoansProvider> logger;
        private readonly IMapper mapper;

        public LoanTests()
        {
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(nameof(BookTests))
                .Options;
            dbContext = new LibraryDbContext(options);

            var loggerMock = new Mock<ILogger<LoansProvider>>();
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
            dbContext.Add(new Db.Book
            {
                Id = 101,
                Title = "Beleza",
                Author = "Roger Scruton",
                Loans = new List<Db.Loan>()
                {
                    new Loan() { User = "Maria", Borrowed = DateTime.Now }
                }
            });
            dbContext.Add(new Db.Book
            {
                Id = 102,
                Title = "O Homem Eterno",
                Author = "G. K. Chesterton",
                Loans = new List<Db.Loan>()
                {
                    new Loan() { User = "João", Borrowed = DateTime.Now.AddDays(-2), Returned = DateTime.Now.AddDays(-1) },
                    new Loan() { User = "Jaison", Borrowed = DateTime.Now }
                }
            });

            dbContext.SaveChanges();
            dbContext.ChangeTracker.Clear();
        }

        [TestMethod]
        public async Task GetAllLoansByBook_Success()
        {
            var loansProvider = new LoansProvider(dbContext, logger, mapper);

            var result = await loansProvider.GetLoansAsync(102);

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Loans);          
            Assert.IsNull(result.ErrorMessage);
        }

        [TestMethod]
        public async Task GetAllLoansByBook_Fail()
        {
            var loansProvider = new LoansProvider(dbContext, logger, mapper);

            var result = await loansProvider.GetLoansAsync(105);

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNull(result.Loans);
            Assert.IsNotNull(result.ErrorMessage);
        }

        [TestMethod]
        public async Task CreateLoan_Success()
        {
            var booksProvider = new LoansProvider(dbContext, logger, mapper);

            var result = await booksProvider.CreateLoanAsync(new Models.Loan()
            {
                BookId = 100,
                User = "Carlos",
                Borrowed = DateTime.Now
            });

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.Loan);
            Assert.IsNull(result.ErrorMessage);
        }

        [TestMethod]
        public async Task CreateLoan_Fail_AlreadyBorrowed()
        {
            var booksProvider = new LoansProvider(dbContext, logger, mapper);

            var result = await booksProvider.CreateLoanAsync(new Models.Loan()
            {
                BookId = 102,
                User = "Carlos",
                Borrowed = DateTime.Now
            });

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNull(result.Loan);
            Assert.IsNotNull(result.ErrorMessage);
            Assert.IsTrue(result.ErrorMessage == "Book not available");
        }

        [TestMethod]
        public async Task CreateLoan_Fail_NotFound()
        {
            var booksProvider = new LoansProvider(dbContext, logger, mapper);

            var result = await booksProvider.CreateLoanAsync(new Models.Loan()
            {
                BookId = 105,
                User = "Carlos",
                Borrowed = DateTime.Now
            });

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNull(result.Loan);
            Assert.IsNotNull(result.ErrorMessage);
            Assert.IsTrue(result.ErrorMessage == "Book not found");
        }
    }
}
