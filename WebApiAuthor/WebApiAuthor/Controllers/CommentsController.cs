using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthor.DTOs;
using WebApiAuthor.Entities;

namespace WebApiAuthor.Controllers;

[ApiController]
[Route("api/books/{bookId:int}/comments")]
public class CommentsController : ControllerBase
{
      private readonly ApplicationDbContext _dbContext;
      private readonly IMapper _mapper;

      public CommentsController(ApplicationDbContext dbContext, IMapper mapper)
      {
            _dbContext = dbContext;
            _mapper = mapper;
      }

      [HttpGet]
      public async Task<ActionResult<List<CommentDTO>>> Get(int bookId)
      {
          var bookExists = await _dbContext.Books.AnyAsync(bookDb => bookDb.Id == bookId);

          if (!bookExists) return NotFound();
          
          var comments = await _dbContext.Comments.Where(
              commentDB => commentDB.BookId == bookId).ToListAsync();

          return _mapper.Map<List<CommentDTO>>(comments);
      }
      

      [HttpPost]
      public async Task<ActionResult> Post(int bookId, CommentCreationDTO commentCreationDto)
      {
          var bookExists = await _dbContext.Books.AnyAsync(bookDb => bookDb.Id == bookId);

          if (!bookExists) return NotFound();

          var comment = _mapper.Map <Comment>(commentCreationDto);
          comment.BookId = bookId;
          _dbContext.Add(comment);
          await _dbContext.SaveChangesAsync();
          return Ok();
      }
}