using CalendarNotes.API.Mappings;
using CalendarNotes.BLL.Mappings;
using CalendarNotes.BLL.Services;
using CalendarNotes.BLL.Services.Interfaces;
using CalendarNotes.DAL.Contexts;
using CalendarNotes.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.OData;
using OData.Swagger.Services;
using CalendarNotes.API.Helpers;

namespace CalendarNotes.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(configs =>
            {
                configs.EnableAnnotations();

                configs.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Note API",
                    Version = "v1",
                    Description = "An API to perform operations with notes",
                    Contact = new OpenApiContact
                    {
                        Name = "Bagrat Antonyan",
                        Email = "bag.antonyan@gmail.com",
                        Url = new Uri("https://github.com/bagantonyan")
                    }
                });

                configs.OperationFilter<EnableODataQueryOptions>();
            });
        }

        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CalendarNotesDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<INoteService, NoteService>();

            services.AddAutoMapper(config =>
            {
                config.AddProfile<APIMappingProfile>();
                config.AddProfile<BLLMappingProfile>();
            });
        }

        public static void AddCorsConfigs(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }
    }
}