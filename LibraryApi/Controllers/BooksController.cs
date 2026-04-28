using LibraryApi.Data;
using LibraryApi.DTOs;
using LibraryApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly LibraryDBContext _context;

        public BooksController(LibraryDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        public ActionResult<IEnumerable<BookDto>> GetAll([FromQuery] string? category, [FromQuery] int? year)
        {
            var query = _context.Book.Include(b => b.Author).AsQueryable();

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(b => b.Category == category);
            }

            if (year.HasValue)
            {
                query = query.Where(query => query.Year == year.Value);
            }

            var books = query.Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Year = b.Year,
                Category = b.Category,
                AuthorName = b.Author!.FullName
            }).ToList();

            return Ok(books);
        }

        [HttpGet("{id}")]
        public ActionResult<BookDto> GetById([FromQuery] int id)
        {
            var book = _context.Book.Include(b => b.Author).FirstOrDefault(x => x.Id == id);

            if (book == null)
                return NotFound(new { error = $"Book with id {id} not found" });

            var dto = new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Category = book.Category,
                AuthorName = book.Author!.FullName,
                Year = book.Year,
            };

            return Ok(dto);


        }

        [HttpPost]
        public ActionResult<BookDto> Create([FromBody] BookCreateDto bookCreateDto)
        {
            if (string.IsNullOrWhiteSpace(bookCreateDto.Title))
                return BadRequest(new { error = "Title is required" }); // 400

            if (bookCreateDto.Year < 1000 || bookCreateDto.Year > DateTime.Now.Year)
                return BadRequest(new { error = "Year is not valid" }); // 400

            var authorExists = _context.Authors.Any(a => a.Id == bookCreateDto.AuthorId);
            if (!authorExists)
                return BadRequest(new { error = $"Author with id {bookCreateDto.AuthorId} not found" }); // 400

            var book = new Book
            {
                Title = bookCreateDto.Title,
                Year = bookCreateDto.Year,
                Category = bookCreateDto.Category,
                AuthorId = bookCreateDto.AuthorId
            };

            _context.Book.Add(book);
            _context.SaveChanges();

            _context.Entry(book).Reference(b => b.Author).Load();

            var result = new BookDto
            {
             
                Title = book.Title,
                Year = book.Year,
                Category = book.Category,
                AuthorName = book.Author!.FullName

            };

            return CreatedAtAction(nameof(GetById), new { id = book.Id }, result); // 201 created
        }

        [HttpPut("{id}")]
        public ActionResult<BookUpdateDto> Update(int id, [FromBody] BookUpdateDto updateDto)
        {
            var book = _context.Book.Find(id);
            if (book == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(updateDto.Title))
                return BadRequest(new { error = "Title is required" }); //400

            book.Title = updateDto.Title;
            book.Year = updateDto.Year;
            book.Category = updateDto.Category;
            book.AuthorId = updateDto.AuthorId;

            _context.SaveChanges();

            return NoContent(); //204
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var book = _context.Book.Find(id);
            if (book == null)
                return NotFound();

            _context.Book.Remove(book);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
