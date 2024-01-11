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
   public async Task<ActionResult<BookDTO>> GetBook(int id)
   {
       var book = await _context.Books.FirstOrDefaultAsync(x => x.Id == id);
       return _mapper.Map<BookDTO>(book);
   }

   [HttpPost]
   public async Task<ActionResult> Post(BookCreationDTO bookCreationDto)
   {
       /*var exists = await _context.Authors.AnyAsync(x => x.Id == book.AuthorId);

       if (!exists) return BadRequest($"There is no id Author: {book.AuthorId}");*/

       var book = _mapper.Map<Book>(bookCreationDto);
       
       _context.Add(book);
       await _context.SaveChangesAsync();
       return Ok();
   }
}