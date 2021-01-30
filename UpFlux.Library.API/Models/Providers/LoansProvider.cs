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
    public class LoansProvider : ILoansProvider
    {
        private readonly LibraryDbContext dbContext;
        private readonly ILogger<LoansProvider> logger;
        private readonly IMapper mapper;

        public LoansProvider(LibraryDbContext dbContext, ILogger<LoansProvider> logger,
            IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;
        }

        public async Task<(bool IsSuccess, IEnumerable<Loan> Loans, string ErrorMessage)> GetLoansAsync(int bookId)
        {
            try
            {
                var loans = await dbContext.Loans.Where(l => l.BookId == bookId).ToListAsync();
                if (loans != null && loans.Any())
                {
                    var result = mapper.Map<IEnumerable<Db.Loan>, IEnumerable<Models.Loan>>(loans);
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

        public async Task<(bool IsSuccess, Loan Loan, string ErrorMessage)> CreateLoanAsync(Loan loan)
        {
            try
            {
                if (!(await dbContext.Books.AnyAsync(b => b.Id == loan.BookId)))
                    return (false, null, "Book not found");
                if (await dbContext.Loans.AnyAsync(l => l.BookId == loan.BookId))
                    return (false, null, "Book not available");

                loan.Borrowed = DateTime.Now;
                var entity = mapper.Map<Models.Loan, Db.Loan>(loan);
                await dbContext.AddAsync(entity);
                await dbContext.SaveChangesAsync();

                var result = mapper.Map<Db.Loan, Models.Loan>(entity);
                return (true, result, null);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }       
    }
}
