/*using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthor.Entities;
using WebApiAuthor.Filters;
using WebApiAuthor.Services;

namespace WebApiAuthor.Controllers;

[ApiController]    
[Route("api/test")] 
public class TestClass : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IService _service;
    private readonly TransientService _transientService;
    private readonly ScopeService _scopeService;
    private readonly SingletonService _singletonService;
    private readonly ILogger<AuthorsController> _logger;

    public TestClass(ApplicationDbContext context, IService service, TransientService transientService,
        ScopeService scopeService, SingletonService singletonService, ILogger<AuthorsController> logger)
    {
        _context = context;
        _service = service;
        _transientService = transientService;
        _scopeService = scopeService;
        _singletonService = singletonService;
        _logger = logger;
    }

    [HttpGet("GUID")]
    //[ResponseCache(Duration = 10)]      //Duracion de 10 segundos, se guarda en la memoria Cache.
    [ServiceFilter(typeof(ActionFilter))]
    public ActionResult GetGuids()
    {
        return Ok(new
        {
            TransientAuthorsController = _transientService.Guid,
            Aservice_Transient = _service.GetTransient(),
            ScopedAuthorsController = _scopeService.Guid,    
            Aservice_Scoped = _service.GetScoped(),
            SingletonAuthorsController = _singletonService.Guid,
            Aservice_Singleton = _service.GetSingleton()
            
        });
    }
    
    [HttpGet] //api/authors
    [HttpGet("listing")] //api/authors/listing
    [HttpGet("/listing")]  //listing    
    [ServiceFilter(typeof(ActionFilter))]
    public async Task<ActionResult<List<Author>>> GetAuthors()
    {
        //throw new NotImplementedException();
        _logger.LogInformation("Getting the Authors");
        _logger.LogWarning("A Test Message");
        _service.DoTask();
        return await _context.Authors.Include(x => x.Books).ToListAsync();
    }

    [HttpGet("first")]    //api/authors/first
    public async Task<ActionResult<Author>> FirstAuthor()
    {
        return await _context.Authors.FirstOrDefaultAsync();
    }

    [HttpGet("{id:int}/{param2?}")]
    public async Task<ActionResult<Author>> GetById(int id, string param2)
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
}*/