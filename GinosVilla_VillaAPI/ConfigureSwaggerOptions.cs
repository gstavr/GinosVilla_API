using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GinosVilla_VillaAPI
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        readonly IApiVersionDescriptionProvider _apiVersionDescriptionProvider;
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider apiVersionDescriptionProvider) => _apiVersionDescriptionProvider = apiVersionDescriptionProvider;
        
        public void Configure(SwaggerGenOptions options)
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Description =
                    "JWT Authorization header using the Bearer scheme. \r\n\r\n" +
                    "Enter the 'Bearer' [space] and then your token in the text input below. \r\n\r\n" +
                    "Example: \"Bearer 1234asdfad\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name =  "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

            foreach (var desc in _apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(desc.GroupName, new OpenApiInfo()
                {
                    Version = $"v{desc.ApiVersion.ToString()}",
                    Title = $"Ginos Villa {desc.ApiVersion}",
                    Description = "API to manage Ginos Villa",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact()
                    {
                        Name = "Ginos",
                        Url = new Uri("https://github.com/gstavr/GinosVilla_API")
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "Example License",
                        Url = new Uri("https://example.com/terms"),
                    }
                });
            }

            //options.SwaggerDoc("v1", new OpenApiInfo()
            //{
            //    Version = "v1.0",
            //    Title = "Ginos Villa V1",
            //    Description = "API to manage Ginos Villa",
            //    TermsOfService = new Uri("https://example.com/terms"),
            //    Contact = new OpenApiContact()
            //    {
            //        Name = "Ginos",
            //        Url = new Uri("https://github.com/gstavr/GinosVilla_API")
            //    },
            //    License = new OpenApiLicense()
            //    {
            //        Name = "Example License",
            //        Url = new Uri("https://example.com/terms"),
            //    }
            //});
            //options.SwaggerDoc("v2", new OpenApiInfo()
            //{
            //    Version = "v2.0",
            //    Title = "Ginos Villa V2",
            //    Description = "API to manage Ginos Villa",
            //    TermsOfService = new Uri("https://example.com/terms"),
            //    Contact = new OpenApiContact()
            //    {
            //        Name = "Ginos",
            //        Url = new Uri("https://github.com/gstavr/GinosVilla_API")
            //    },
            //    License = new OpenApiLicense()
            //    {
            //        Name = "Example License",
            //        Url = new Uri("https://example.com/terms"),
            //    }
            //});
        }
    }
}
