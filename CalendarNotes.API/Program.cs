using CalendarNotes.API.Extensions;
using CalendarNotes.API.Filters;
using CalendarNotes.API.Handlers;
using CalendarNotes.API.Hubs;
using CalendarNotes.API.Services;
using CalendarNotes.BLL.Mappings;
using CalendarNotes.DAL.Contexts;
using CalendarNotes.DAL.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using OData.Swagger.Services;
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
            builder.Services.AddProblemDetails();

            builder.Services.AddCorsConfigs();

            builder.Services.AddSignalR();

            builder.Services.AddHostedService<NotificationService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwagger();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.AddSwagger();
            }

            app.UseExceptionHandler();

            app.UseHttpsRedirection();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            await app.MigrateDatabaseAsync();

            app.MapControllers();

            app.MapHub<NotificationHub>("/notificationHub");

            app.Run();
        }
    }
}
