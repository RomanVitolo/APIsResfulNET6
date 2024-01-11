using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthor.DTOs;
using WebApiAuthor.Entities;
using WebApiAuthor.Filters;
using WebApiAuthor.Services;

namespace WebApiAuthor.Controllers;

[ApiController]
[Route("api/authors")]  //This is the Path     

public class AuthorsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthorsController> _logger;

    public AuthorsController(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }    
    
    [HttpGet] //api/authors    
    public async Task<ActionResult<List<AuthorDTO>>> GetAuthors()
    {     
        var authors = await _dbContext.Authors.ToListAsync();
        return _mapper.Map<List<AuthorDTO>>(authors);
    }               

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AuthorDTO>> GetById(int id)
    {
        var author = await _dbContext.Authors.FirstOrDefaultAsync(authorBD => authorBD.Id == id);
        if (author == null)  
            return NotFound();

        return _mapper.Map<AuthorDTO>(author);      
    }
    
    [HttpGet("{name}")]
    public async Task<ActionResult<List<AuthorDTO>>> GetByName([FromRoute] string name)
    {
        var authors = await _dbContext.Authors.Where
            (authorBD => authorBD.Name.Contains(name)).ToListAsync();    

        return _mapper.Map<List<AuthorDTO>>(authors);
    }

    [HttpPost]
    public async Task<ActionResult> PostAuthors([FromBody] AuthorCreationDTO authorCreationDto)  //Mostrar esta propiedad no es lo correcto
    {
        var existsSameName = await _dbContext.Authors.AnyAsync(x => x.Name == authorCreationDto.Name);
        if (existsSameName)
        {
            return BadRequest($"An author with the same name already exists {authorCreationDto.Name}");
        }

        var author = _mapper.Map<Author>(authorCreationDto);
        
        _dbContext.Add(author);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("{id:int}")] //api/authors/1
    public async Task<ActionResult> PutAuthor(Author author, int id)
    {
        if (author.Id != id)
        {
            return BadRequest("The author id does not match the id of the URL");
        }
        
        var exists = await _dbContext.Authors.AnyAsync(x => x.Id == id);
        if (exists)
            return NotFound();        

        _dbContext.Update(author);
        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id:int}")] //api/authors/2
    public async Task<ActionResult> DeleteAuthor(int id)
    {
        var exists = await _dbContext.Authors.AnyAsync(x => x.Id == id);
        if (exists)    
            return NotFound();

        _dbContext.Remove(new Author() {Id = id});
        await _dbContext.SaveChangesAsync();
        return Ok();             
    }
}