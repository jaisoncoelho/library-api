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
    [Route("books")]
    public class BooksController : ControllerBase
    {
        private readonly IBooksProvider booksProvider;

        public BooksController(IBooksProvider booksProvider)
        {
            this.booksProvider = booksProvider;
        }

        [HttpGet]
        public async Task<IActionResult> GetBooksAsync([FromQuery] string title, [FromQuery] string author)
        {
            var result = await booksProvider.GetBooksAsync(title, author);
            if (result.IsSuccess)
                return Ok(result.Books);

            return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookAsync(int id)
        {
            var result = await booksProvider.GetBookAsync(id);
            if (result.IsSuccess)
                return Ok(result.Book);

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBookAsync([FromBody] Book book)
        {
            var result = await booksProvider.CreateBookAsync(book);
            if (result.IsSuccess)
                return Ok(result.Book);

            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBookAsync([FromBody] Book book, int id)
        {
            book.Id = id;
            var result = await booksProvider.UpdateBookAsync(book);
            if (result.IsSuccess)
                return Ok(result.Book);

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveBookAsync(int id)
        {
            var result = await booksProvider.RemoveBookAsync(id);
            if (result.IsSuccess)
                return Ok();

            return NotFound();
        }
    }
}
