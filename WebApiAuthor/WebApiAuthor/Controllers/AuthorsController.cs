using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthor.Entities;
using WebApiAuthor.Filters;
using WebApiAuthor.Services;

namespace WebApiAuthor.Controllers;

[ApiController]
[Route("api/authors")]  //This is the Path     

public class AuthorsController : ControllerBase
{
    private readonly ApplicationDbContext _context;  
    private readonly ILogger<AuthorsController> _logger;

    public AuthorsController(ApplicationDbContext context)
    {
        _context = context;  
    }    
    
    [HttpGet] //api/authors    
    public async Task<ActionResult<List<Author>>> GetAuthors()
    {     
        return await _context.Authors.ToListAsync();
    }               

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Author>> GetById(int id)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(x => x.Id == id);
        if (author == null)  
            return NotFound();

        return author;
    }
    
    [HttpGet("{name}")]
    public async Task<ActionResult<Author>> GetByName(string name)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(x => x.Name.Contains(name));
        if (author == null)  
            return NotFound();

        return author;
    }

    [HttpPost]
    public async Task<ActionResult> PostAuthors([FromBody] Author author)
    {
        var existsSameName = await _context.Authors.AnyAsync(x => x.Name == author.Name);
        if (existsSameName)
        {
            return BadRequest($"An author with the same name already exists {author.Name}");
        }
        
        _context.Add(author);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("{id:int}")] //api/authors/1
    public async Task<ActionResult> PutAuthor(Author author, int id)
    {
        if (author.Id != id)
        {
            return BadRequest("The author id does not match the id of the URL");
        }
        
        var exists = await _context.Authors.AnyAsync(x => x.Id == id);
        if (exists)
            return NotFound();        

        _context.Update(author);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id:int}")] //api/authors/2
    public async Task<ActionResult> DeleteAuthor(int id)
    {
        var exists = await _context.Authors.AnyAsync(x => x.Id == id);
        if (exists)    
            return NotFound();

        _context.Remove(new Author() {Id = id});
        await _context.SaveChangesAsync();
        return Ok();             
    }
}