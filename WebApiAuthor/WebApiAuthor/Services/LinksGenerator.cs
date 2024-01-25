using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using WebApiAuthor.DTOs;

namespace WebApiAuthor.Services;

public class LinksGenerator
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IActionContextAccessor _actionContextAccessor;

    public LinksGenerator(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor,
        IActionContextAccessor actionContextAccessor)
    {
        _authorizationService = authorizationService;
        _httpContextAccessor = httpContextAccessor;
        _actionContextAccessor = actionContextAccessor;
    }

    private IUrlHelper BuildUrlHelper()
    {
        var factory = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
        return factory.GetUrlHelper(_actionContextAccessor.ActionContext);
    }

    private async Task<bool> IsAdmin()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var result = await _authorizationService.AuthorizeAsync(httpContext.User, "isAdmin");
        return result.Succeeded;
    }

    public async Task LinkGenerator(AuthorDTO authorDto)
    {
        var isAdmin = await IsAdmin();
        var Url = BuildUrlHelper();

        authorDto.Links.Add(new HATEOASDate(link: Url.Link("getAuthor",
                new { id = authorDto.Id }),
            description: "self", method: "GET"));

        if (isAdmin)
        {
            authorDto.Links.Add(new HATEOASDate(link: Url.Link("refreshAuthor",
                    new { id = authorDto.Id }),
                description: "refresh-author", method: "PUT"));

            authorDto.Links.Add(new HATEOASDate(link: Url.Link("deleteAuthor",
                    new { id = authorDto.Id }),
                description: "delete-author", method: "DELETE"));
        }
    }
}