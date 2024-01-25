using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace WebApiAuthors.Tests.Mocks
{
    public class URLHelperMock : IUrlHelper
    {
        public string? Action(UrlActionContext actionContext)
        {
            throw new NotImplementedException();
        }

        public string? Content(string? contentPath)
        {
            throw new NotImplementedException();
        }

        public bool IsLocalUrl(string? url)
        {
            throw new NotImplementedException();
        }

        public string? RouteUrl(UrlRouteContext routeContext)
        {
            throw new NotImplementedException();
        }

        public string Link(string? routeName, object? values)
        {
            return "";
        }

        public ActionContext ActionContext { get; }
    }
}

