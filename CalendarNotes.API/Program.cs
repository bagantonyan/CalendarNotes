using CalendarNotes.API.Extensions;
using CalendarNotes.API.Filters;
using CalendarNotes.API.Handlers;
using CalendarNotes.API.Hubs;
using CalendarNotes.API.Services;
using CalendarNotes.DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using Serilog;

namespace CalendarNotes.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();

            ODataConventionModelBuilder odataBuilder = new ODataConventionModelBuilder();
            odataBuilder.EntitySet<Note>("Notes");

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add(new GlobalResponseFilter());
                options.Filters.Add(new ProducesAttribute("application/json"));
            }).AddOData(opt => opt
                .Select()
                .Filter()
                .OrderBy()
                .Expand()
                .SetMaxTop(100)
                .Count());

            builder.Services.AddServices(builder.Configuration);

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            builder.Services.AddFluentValidation();

            builder.Services.AddProblemDetails();

            builder.Services.AddCorsConfigs();
            
            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.Services.AddSignalR();

            builder.Services.AddHostedService<NotificationService>();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwagger();


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.AddSwagger();
            }

            app.UseExceptionHandler();

            app.UseHttpsRedirection();

            app.UseCors("CorsPolicy");
            
            app.UseAuthentication();
            app.UseAuthorization();

            await app.MigrateDatabaseAsync();

            app.MapControllers();

            app.MapHub<NotificationHub>("/notificationHub");
            app.MapHub<ChatHub>("/chatHub");

            app.Run();
        }
    }
}