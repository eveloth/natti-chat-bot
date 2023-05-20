using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using NattiChatBot.Filters;

namespace NattiChatBot.Installers;

public static class SwaggerInstaller
{
    public static void InstallSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(options =>
        {
            var xmlCommentsFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlCommentsFileName));
            options.OperationFilter<AddHeader>();
        });
    }
}

public class AddHeader : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (
            context.MethodInfo
                .GetCustomAttributes(typeof(ValitatePfzTokenAttribute))
                .FirstOrDefault() is ValitatePfzTokenAttribute
        )
        {
            operation.Parameters.Add(
                new OpenApiParameter
                {
                    Name = "X-Pfz-Token",
                    In = ParameterLocation.Header,
                    Required = false,
                    Schema = new OpenApiSchema { Type = "string" }
                }
            );
        }
    }
}