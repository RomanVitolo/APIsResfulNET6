using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAuthor.DTOs;

namespace WebApiAuthor.Controllers.V1;

[ApiController]
[Route("api/v1")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class RootController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;

    public RootController(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    [HttpGet(Name = "getRoot")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<HATEOASDate>>> Get()
    {
        var hateoasDate = new List<HATEOASDate>();

        var isAdmin = await _authorizationService.AuthorizeAsync(User, "isAdmin");

        hateoasDate.Add(new HATEOASDate(link: Url.Link("getRoot", new { }),
            description: "self", method: "GET"));

        hateoasDate.Add(new HATEOASDate(link: Url.Link("getAuthors", new { }),
            description: "authors", method: "GET"));

        if (isAdmin.Succeeded)
        {
            hateoasDate.Add(new HATEOASDate(link: Url.Link("createAuthor", new { }),
                description: "create-author", method: "POST"));

            hateoasDate.Add(new HATEOASDate(link: Url.Link("createBook", new { }),
                description: "create-book", method: "POST"));
        }

        return hateoasDate;
    }
}