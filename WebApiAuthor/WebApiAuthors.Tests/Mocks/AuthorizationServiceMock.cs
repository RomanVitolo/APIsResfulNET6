using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace WebApiAuthors.Tests.Mocks
{
    public class AuthorizationServiceMock : IAuthorizationService
    {
        public AuthorizationResult Result { get; set; }

        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource,
            IEnumerable<IAuthorizationRequirement> requirements)
        {
            return Task.FromResult(Result);
        }

        public Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, string policyName)
        {
            return Task.FromResult(Result);
        }
    }
}

