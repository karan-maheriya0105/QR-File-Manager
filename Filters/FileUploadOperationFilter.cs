using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace BrochureAPI.Filters
{
    public class FileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileParameters = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile) || 
                           (p.ParameterType.IsGenericType && p.ParameterType.GetGenericArguments()[0] == typeof(IFormFile)))
                .ToList();

            if (fileParameters.Count == 0)
                return;

            // Add consumes/content type for multipart/form-data
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = fileParameters.ToDictionary(
                                prop => prop.Name ?? string.Empty,
                                prop => new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary"
                                }
                            ),
                            Required = new HashSet<string>(fileParameters.Select(p => p.Name ?? string.Empty))
                        }
                    }
                },
                Required = true
            };
        }
    }
}
