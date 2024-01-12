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

      [HttpGet("{id:int}", Name = "getComment")]
      public async Task<ActionResult<CommentDTO>> GetById(int id)
      {
          var comment = await _dbContext.Comments
              .FirstOrDefaultAsync(commentDB => commentDB.Id == id);

          if (comment == null) return NotFound();

          return _mapper.Map<CommentDTO>(comment);
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

          var commentDTO = _mapper.Map<CommentDTO>(comment);
          

          return CreatedAtRoute("getComment", 
              new {id = comment.Id, bookId = bookId}, commentDTO);
      }

      [HttpPut("{id:int}")]
      public async Task<ActionResult> PutComment(int bookId, int id, CommentCreationDTO commentCreationDto)
      {
          var existBook = await _dbContext.Books.AnyAsync(bookDB => bookDB.Id == bookId);  
          if (!existBook) return NotFound();

          var existComment = await _dbContext.Comments.AnyAsync(commentDB => commentDB.Id == id);  
          if (!existComment) return NotFound();

          var comment = _mapper.Map<Comment>(commentCreationDto);
          comment.Id = id;
          comment.BookId = bookId;
          
          _dbContext.Update(comment);
          await _dbContext.SaveChangesAsync();
          return NoContent();    
      }
}