using CalendarNotes.API.Mappings;
using CalendarNotes.BLL.Mappings;
using CalendarNotes.BLL.Services;
using CalendarNotes.BLL.Services.Interfaces;
using CalendarNotes.DAL.Contexts;
using CalendarNotes.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using CalendarNotes.API.Helpers;
using FluentValidation.AspNetCore;
using FluentValidation;
using CalendarNotes.API.ModelValidators.Notes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
                
                // Добавляем поддержку JWT в Swagger
                configs.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                configs.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }

        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Отключаем конвертацию DateTime в UTC для PostgreSQL
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            
            services.AddDbContext<CalendarNotesDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<INoteService, NoteService>();
            services.AddScoped<IChatService, ChatService>();

            services.AddAutoMapper(config =>
            {
                config.AddProfile<APIMappingProfile>();
                config.AddProfile<BLLMappingProfile>();
            });
        }

        public static void AddFluentValidation(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(CreateNoteRequestModelValidator).Assembly);
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
        }

        public static void AddCorsConfigs(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed(_ => true) // Разрешаем любые источники в dev режиме
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()); // Важно для SignalR с WebSockets
            });
        }
        
        /// <summary>
        /// Добавляет JWT аутентификацию
        /// </summary>
        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? "ThisIsMyVerySecureSecretKeyForJwtTokenGenerationWhichShouldBeAtLeast32Characters")),
                    ClockSkew = TimeSpan.Zero // Убираем стандартные 5 минут
                };
                
                // Добавляем поддержку SignalR
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        
                        if (!string.IsNullOrEmpty(accessToken) && 
                            (path.StartsWithSegments("/notificationHub") || path.StartsWithSegments("/chatHub")))
                        {
                            context.Token = accessToken;
                        }
                        
                        return Task.CompletedTask;
                    }
                };
            });
            
            // Добавляем политики авторизации
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("RequireUserRole", policy => policy.RequireRole("User", "Admin"));
            });
        }
    }
}