using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UpFlux.Library.API.Interfaces;
using UpFlux.Library.API.Models;

namespace UpFlux.Library.API.Controllers
{
    [ApiController]
    [Route("books/{bookId}/loans")]
    public class LoansController : ControllerBase
    {
        private readonly ILoansProvider loansProvider;

        public LoansController(ILoansProvider loansProvider)
        {
            this.loansProvider = loansProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetLoansAsync(int bookId)
        {
            var result = await loansProvider.GetLoansAsync(bookId);
            if (result.IsSuccess)
                return Ok(result.Loans);

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateLoanAsync([FromBody] Loan loan, int bookId)
        {
            loan.BookId = bookId;
            var result = await loansProvider.CreateLoanAsync(loan);
            if (result.IsSuccess)
                return Ok(result.Loan);

            return BadRequest(result.ErrorMessage);
        }
    }
}
