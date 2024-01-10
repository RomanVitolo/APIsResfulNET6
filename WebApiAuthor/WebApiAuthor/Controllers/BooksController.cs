using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthor.Entities;

namespace WebApiAuthor.Controllers;

[ApiController]
[Route("api/Books")]
public class BooksController : ControllerBase
{
   private readonly ApplicationDbContext _context;

   public BooksController(ApplicationDbContext context)
   {
      _context = context;
   }

   /*[HttpGet("{id:int}")]
   public async Task<ActionResult<Book>> GetBook(int id)
   {
       return await _context.Books.Include
           (x => x.Author).FirstOrDefaultAsync(x => x.Id == id);
   }

   [HttpPost]
   public async Task<ActionResult> Post(Book book)
   {
       var exists = await _context.Authors.AnyAsync(x => x.Id == book.AuthorId);

       if (!exists) return BadRequest($"There is no id Author: {book.AuthorId}");

       _context.Add(book);
       await _context.SaveChangesAsync();
       return Ok();
   }*/
}