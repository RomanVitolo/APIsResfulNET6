using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthor.DTOs;
using WebApiAuthor.Entities;
using WebApiAuthor.Utilities;

namespace WebApiAuthor.Controllers.V1
{
    [ApiController]
[Route("api/v1/books/{bookId:int}/comments")]
public class CommentsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly UserManager<IdentityUser> _userManager;

    public CommentsController(ApplicationDbContext dbContext, IMapper mapper, UserManager<IdentityUser> userManager)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userManager = userManager;
    }

    [HttpGet(Name = "getBookComments")]
    public async Task<ActionResult<List<CommentDTO>>> Get(int bookId, [FromQuery] PageDTO pageDto)
    {
        var bookExists = await _dbContext.Books.AnyAsync(bookDb => bookDb.Id == bookId);

        if (!bookExists) return NotFound();

        var queryable = _dbContext.Comments.Where(
            commentDB => commentDB.BookId == bookId).AsQueryable();
        await HttpContext.InsertPagingParametersInHeader(queryable);

        var comments = await queryable.OrderBy(comment => comment.Id)
            .Page(pageDto).ToListAsync();

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


    [HttpPost(Name = "createComment")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Post(int bookId, CommentCreationDTO commentCreationDto)
    {
        var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
        var email = emailClaim.Value;
        var user = await _userManager.FindByEmailAsync(email);
        var userId = user.Id;

        var bookExists = await _dbContext.Books.AnyAsync(bookDb => bookDb.Id == bookId);

        if (!bookExists) return NotFound();

        var comment = _mapper.Map<Comment>(commentCreationDto);
        comment.BookId = bookId;
        comment.UserId = userId;
        _dbContext.Add(comment);
        await _dbContext.SaveChangesAsync();

        var commentDTO = _mapper.Map<CommentDTO>(comment);


        return CreatedAtRoute("getComment",
            new { id = comment.Id, bookId = bookId }, commentDTO);
    }

    [HttpPut("{id:int}", Name = "refreshComment")]
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
}

