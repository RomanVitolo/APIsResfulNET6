using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthor.DTOs;
using WebApiAuthor.Entities;
using WebApiAuthor.Utilities;

namespace WebApiAuthor.Controllers;

[ApiController]
[Route("api/authors")]  //This is the Path     
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
public class AuthorsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<AuthorsController> _logger;

    public AuthorsController(ApplicationDbContext dbContext, IMapper mapper, IAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _authorizationService = authorizationService;
    }

    /*[HttpGet("Configurations")]
    public ActionResult<string> GetConfigurations()
    {
        return _configuration["lastName"];
    }*/
    
    
    [HttpGet(Name = "getAuthors")] //api/authors 
    [AllowAnonymous]
    [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
    public async Task<ActionResult<List<AuthorDTO>>> GetAuthors() //[FromHeader] string includeHATEOAS)
    {     
        var authors = await _dbContext.Authors.ToListAsync();
        return _mapper.Map<List<AuthorDTO>>(authors);   
    }               

    [HttpGet("{id:int}", Name = "getAuthor")]
    [AllowAnonymous]   
    [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
    public async Task<ActionResult<AuthorDTOWithBooks>> GetById(int id) //, [FromHeader] string includeHATEOAS)
    {
        var author = await _dbContext.Authors
            .Include(authorDB => authorDB.AuthorsBooks)
            .ThenInclude(authorBookDB => authorBookDB.Book)
            .FirstOrDefaultAsync(authorBD => authorBD.Id == id);
        
        if (author == null)  
            return NotFound();

        var dto = _mapper.Map<AuthorDTOWithBooks>(author);       
        return dto;
    }         
    
    
    [HttpGet("{name}", Name = "getAuthorByName")]
    public async Task<ActionResult<List<AuthorDTO>>> GetByName([FromRoute] string name)
    {
        var authors = await _dbContext.Authors.Where
            (authorBD => authorBD.Name.Contains(name)).ToListAsync();    

        return _mapper.Map<List<AuthorDTO>>(authors);
    }

    [HttpPost(Name = "createAuthor")]
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

        var authorDTO = _mapper.Map<AuthorDTO>(author);
        return CreatedAtRoute("getAuthor", new { id = author.Id},authorDTO);
    }

    [HttpPut("{id:int}", Name = "refreshAuthor")] //api/authors/1
    public async Task<ActionResult> PutAuthor(AuthorCreationDTO authorCreationDto, int id)
    {       
        var exists = await _dbContext.Authors.AnyAsync(x => x.Id == id);
        if (!exists)
            return NotFound();

        var author = _mapper.Map<Author>(authorCreationDto);
        author.Id = id;

        _dbContext.Update(author);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}", Name = "deleteAuthor")] //api/authors/2
    public async Task<ActionResult> DeleteAuthor(int id)
    {
        var exists = await _dbContext.Authors.AnyAsync(x => x.Id == id);
        if (!exists)    
            return NotFound();

        _dbContext.Remove(new Author() {Id = id});
        await _dbContext.SaveChangesAsync();
        return NoContent();             
    }
}