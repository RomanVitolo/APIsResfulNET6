using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthor.DTOs;
using WebApiAuthor.Entities;

namespace WebApiAuthor.Controllers;

[ApiController]
[Route("api/Books")]
public class BooksController : ControllerBase
{
   private readonly ApplicationDbContext _context;
   private readonly IMapper _mapper;

   public BooksController(ApplicationDbContext context, IMapper mapper)
   {
       _context = context;
       _mapper = mapper;
   }

   [HttpGet("{id:int}")]
   public async Task<ActionResult<BookDTOWithAuthors>> GetBook(int id)
   {
       var book = await _context.Books
           .Include(bookDB => bookDB.Comments)
           .Include(bookDB => bookDB.AuthorsBooks).ThenInclude(authorBookDB => authorBookDB.Author)
           .FirstOrDefaultAsync(x => x.Id == id);

       book.AuthorsBooks = book.AuthorsBooks.OrderBy(x => x.Order).ToList();
       
       return _mapper.Map<BookDTOWithAuthors>(book);
   }

   [HttpPost]
   public async Task<ActionResult> Post(BookCreationDTO bookCreationDto)
   {
       if (bookCreationDto.AuthorsIds == null) return BadRequest("Cannot create a book without Authors");   
       
       var authorsIds = await _context.Authors.Where
           (authorDB => bookCreationDto.AuthorsIds.Contains(authorDB.Id)).
           Select(author => author.Id).ToListAsync();

       if (bookCreationDto.AuthorsIds.Count != authorsIds.Count)
           return BadRequest("One of the Authors does not exist"); 

       var book = _mapper.Map<Book>(bookCreationDto);

       if (book.AuthorsBooks != null)
       {
           for (int i = 0; i < book.AuthorsBooks.Count; i++)
           {
               book.AuthorsBooks[i].Order = i;
           }  
       }
       _context.Add(book);
       await _context.SaveChangesAsync();
       return Ok();
   }
}