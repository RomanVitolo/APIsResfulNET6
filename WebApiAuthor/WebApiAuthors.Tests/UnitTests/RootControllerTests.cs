using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WebApiAuthor.Controllers.V1;
using WebApiAuthors.Tests.Mocks;

namespace WebApiAuthors.Tests.UnitTests
{ 
    
    [TestClass]
public class RootControllerTests
{
    [TestMethod]
    public async Task IfUserIsAdmin_Get4Links()
    {
        //Preparation
        var authorizationService = new AuthorizationServiceMock();
        authorizationService.Result = AuthorizationResult.Success();
        var rootController = new RootController(authorizationService);
        rootController.Url = new URLHelperMock();

        //Execute
        var result = await rootController.Get();

        //Validation
        Assert.AreEqual(4, result.Value.Count());
    }

    [TestMethod]
    public async Task IfUserIsNotAdmin_Get2Links()
    {
        //Preparation
        var authorizationService = new AuthorizationServiceMock();
        authorizationService.Result = AuthorizationResult.Failed();
        var rootController = new RootController(authorizationService);
        rootController.Url = new URLHelperMock();

        //Execute
        var result = await rootController.Get();

        //Validation
        Assert.AreEqual(2, result.Value.Count());
    }

    [TestMethod]      //Moq Library NugetPackage
    public async Task IfUserIsNotAdmin_Get2Links_UsingMoq()
    {
        //Preparation
        var mockAuthorizationService = new Mock<IAuthorizationService>();
        mockAuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(),
            It.IsAny<object>(),
            It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
            .Returns(Task.FromResult(AuthorizationResult.Failed()));

        mockAuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<string>()))
            .Returns(Task.FromResult(AuthorizationResult.Failed()));

        var mockURLHelper = new Mock<IUrlHelper>();
        mockURLHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>()))
            .Returns(string.Empty);

        var rootController = new RootController(mockAuthorizationService.Object);
        rootController.Url = mockURLHelper.Object;

        //Execute
        var result = await rootController.Get();

        //Validation
        Assert.AreEqual(2, result.Value.Count());
    }
}
}