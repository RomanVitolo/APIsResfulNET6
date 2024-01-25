using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApiAuthor.DTOs;
using WebApiAuthor.Services;

namespace WebApiAuthor.Utilities;

public class HATEOASAuthorFilterAttribute : HATEOASFilterAttribute
{
    private readonly LinksGenerator _linkGenerator;

    public HATEOASAuthorFilterAttribute(LinksGenerator linkGenerator)
    {
        _linkGenerator = linkGenerator;
    }

    public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var mustInclude = MustIncludeHATEOAS(context);

        if (!mustInclude)
        {
            await next();
            return;
        }

        var result = context.Result as ObjectResult;

        var authorDTO = result.Value as AuthorDTO;

        if (authorDTO == null)
        {
            var authorsDTO = result.Value as List<AuthorDTO> ??
                             throw new Exception("An instance of AuthorDTO was expected or List<AuthorDTO>");
            authorsDTO.ForEach(async author => await _linkGenerator.LinkGenerator(author));
            result.Value = authorsDTO;
        }
        else
        {
            await _linkGenerator.LinkGenerator(authorDTO);
        }

        await next();
    }
}