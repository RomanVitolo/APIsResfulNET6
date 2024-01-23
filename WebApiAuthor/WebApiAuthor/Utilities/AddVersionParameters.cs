using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApiAuthor.Utilities;

public class AddVersionParameters : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
            
        if (operation.Parameters == null)
        {
            operation.Parameters = new List<OpenApiParameter>();
        }                     
        
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "x-version",
            In = ParameterLocation.Header,
            Required = true
        });        
    }
}