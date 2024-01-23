using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebApiAuthor.Utilities;

public class SwaggerGroupVersion : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        var namespaceController = controller.ControllerType.Namespace; // Controllers.V1
        var apiVersion = namespaceController.Split('.').Last().ToLower();   //v1
        controller.ApiExplorer.GroupName = apiVersion;
    }
}