using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UpFlux.Library.API.Models;

namespace UpFlux.Library.API.Interfaces
{
    public interface ILoansProvider
    {
        Task<(bool IsSuccess, IEnumerable<Models.Loan> Loans, string ErrorMessage)> GetLoansAsync(int bookId);
        Task<(bool IsSuccess, Models.Loan Loan, string ErrorMessage)> CreateLoanAsync(Models.Loan loan);       
    }
}
