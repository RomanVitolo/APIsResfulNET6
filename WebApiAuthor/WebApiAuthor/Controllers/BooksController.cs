using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
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

   [HttpGet("{id:int}", Name = "getBooks")]
   public async Task<ActionResult<BookDTOWithAuthors>> GetBook(int id)
   {
       var book = await _context.Books
           .Include(bookDB => bookDB.Comments)
           .Include(bookDB => bookDB.AuthorsBooks).ThenInclude(authorBookDB => authorBookDB.Author)
           .FirstOrDefaultAsync(x => x.Id == id);

       if (book == null) return NotFound();

       book.AuthorsBooks = book.AuthorsBooks.OrderBy(x => x.Order).ToList();
       
       return _mapper.Map<BookDTOWithAuthors>(book);
   }

   [HttpPost(Name = "createBook")]
   public async Task<ActionResult> Post(BookCreationDTO bookCreationDto)
   {
       if (bookCreationDto.AuthorsIds == null) return BadRequest("Cannot create a book without Authors");   
       
       var authorsIds = await _context.Authors.Where
           (authorDB => bookCreationDto.AuthorsIds.Contains(authorDB.Id)).
           Select(author => author.Id).ToListAsync();

       if (bookCreationDto.AuthorsIds.Count != authorsIds.Count)
           return BadRequest("One of the Authors does not exist"); 

       var book = _mapper.Map<Book>(bookCreationDto);

       AssignAuthorsOrder(book);
       
       _context.Add(book);
       await _context.SaveChangesAsync();

       var bookDTO = _mapper.Map<BookDTO>(book);
       return CreatedAtRoute("getBooks", new {id = book.Id},bookDTO);
   }

   [HttpPut("{id:int}", Name = "refreshBook")]
   public async Task<ActionResult> Put(int id, BookCreationDTO bookCreationDto)
   {
       var bookDB = await _context.Books
           .Include(x => x.AuthorsBooks)
           .FirstOrDefaultAsync(x => x.Id == id);

       if (bookDB == null) return NotFound();

       bookDB = _mapper.Map(bookCreationDto, bookDB);
       
       AssignAuthorsOrder(bookDB);
       
       await _context.SaveChangesAsync();
       return NoContent();
   }

   [HttpPatch("{id:int}", Name = "bookPatch")]
   public async Task<ActionResult> Patch(int id, JsonPatchDocument<BookPatchDTO> patchDocument)
   {
       if (patchDocument == null) return BadRequest();

       var bookDB = await _context.Books.FirstOrDefaultAsync(x => x.Id == id);

       if (bookDB == null) return NotFound();

       var bookDTO = _mapper.Map<BookPatchDTO>(bookDB);
       
       patchDocument.ApplyTo(bookDTO, ModelState);

       var isValid = TryValidateModel(bookDTO);   
       if (!isValid) return BadRequest(ModelState);

       _mapper.Map(bookDB, bookDB);
       
       await _context.SaveChangesAsync();
       return NoContent();
   }
   
   [HttpDelete("{id:int}", Name = "deleteBook")]
   public async Task<ActionResult> DeleteBook(int id)
   {
       var exists = await _context.Books.AnyAsync(x => x.Id == id);
       if (!exists)    
           return NotFound();

       _context.Remove(new Book() {Id = id});
       await _context.SaveChangesAsync();
       return NoContent();             
   }

   private void AssignAuthorsOrder(Book book)
   {
       if (book.AuthorsBooks != null)
       {
           for (int i = 0; i < book.AuthorsBooks.Count; i++)
           {
               book.AuthorsBooks[i].Order = i;
           }
       }  
   } 
}