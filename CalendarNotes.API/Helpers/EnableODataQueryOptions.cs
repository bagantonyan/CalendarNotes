using Microsoft.AspNetCore.OData.Query;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CalendarNotes.API.Helpers
{
    public class EnableODataQueryOptions : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.MethodInfo.GetCustomAttributes(typeof(EnableQueryAttribute), false).Any())
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "$filter",
                    In = ParameterLocation.Query,
                    Description = "OData filter expression",
                    Schema = new OpenApiSchema { Type = "string" }
                });
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "$select",
                    In = ParameterLocation.Query,
                    Description = "Select specific fields",
                    Schema = new OpenApiSchema { Type = "string" }
                });
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "$orderby",
                    In = ParameterLocation.Query,
                    Description = "Order results",
                    Schema = new OpenApiSchema { Type = "string" }
                });
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "$top",
                    In = ParameterLocation.Query,
                    Description = "Number of records to retrieve",
                    Schema = new OpenApiSchema { Type = "integer", Format = "int32" }
                });
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "$skip",
                    In = ParameterLocation.Query,
                    Description = "Number of records to skip",
                    Schema = new OpenApiSchema { Type = "integer", Format = "int32" }
                });
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "$count",
                    In = ParameterLocation.Query,
                    Description = "Include count of total records",
                    Schema = new OpenApiSchema { Type = "boolean" }
                });
            }
        }
    }
}