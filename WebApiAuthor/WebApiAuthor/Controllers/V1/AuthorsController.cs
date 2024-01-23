using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthor.DTOs;
using WebApiAuthor.Entities;
using WebApiAuthor.Utilities;

namespace WebApiAuthor.Controllers.V1;

[ApiController]
//[Route("api/v1/authors")]  //This is the Path     
[Route("api/authors")] 
[AttributeHeader("x-version", "1")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
//[ApiConventionType(typeof(DefaultApiConventions))]
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
                                       
    [HttpGet(Name = "getAuthorsv1")] //api/authors 
    [AllowAnonymous]
    [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
    public async Task<ActionResult<List<AuthorDTO>>> GetAuthors([FromQuery] PageDTO pageDto) //[FromHeader] string includeHATEOAS)
    {
        var queryable = _dbContext.Authors.AsQueryable();
        await HttpContext.InsertPagingParametersInHeader(queryable);
        var authors = await queryable.OrderBy(author => author.Name).Page(pageDto).ToListAsync();
        return _mapper.Map<List<AuthorDTO>>(authors);   
    }               

    [HttpGet("{id:int}", Name = "getAuthorv1")]
    [AllowAnonymous]   
    [ServiceFilter(typeof(HATEOASAuthorFilterAttribute))]
    [ProducesResponseType(404)]  //Puedo documentar las respuestas que mi Web Api puede retornar
    [ProducesResponseType(200)]
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
    
    
    [HttpGet("{name}", Name = "getAuthorByNamev1")]
    public async Task<ActionResult<List<AuthorDTO>>> GetByName([FromRoute] string name)
    {
        var authors = await _dbContext.Authors.Where
            (authorBD => authorBD.Name.Contains(name)).ToListAsync();    

        return _mapper.Map<List<AuthorDTO>>(authors);
    }

    [HttpPost(Name = "createAuthorv1")]
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
        return CreatedAtRoute("getAuthorv1", new { id = author.Id},authorDTO);
    }

    [HttpPut("{id:int}", Name = "refreshAuthorv1")] //api/authors/1
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

    /// <summary>
    ///  Delete an Author
    /// </summary>
    /// <param name="id">Author ID to Delete</param>
    /// <returns></returns>
    
    [HttpDelete("{id:int}", Name = "deleteAuthorv1")] //api/authors/2
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