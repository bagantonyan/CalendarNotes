using CalendarNotes.API.Extensions;
using CalendarNotes.API.Hubs;
using CalendarNotes.API.Services;
using CalendarNotes.BLL.Mappings;
using CalendarNotes.DAL.Contexts;
using CalendarNotes.DAL.Entities;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using OData.Swagger.Services;

namespace CalendarNotes.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure OData model
            ODataConventionModelBuilder odataBuilder = new ODataConventionModelBuilder();
            odataBuilder.EntitySet<Note>("Notes");

            builder.Services.AddControllers().AddOData(opt =>
                opt
                .Select()
                .Filter()
                .OrderBy()
                .Expand()
                .SetMaxTop(100)
                .Count()
                .AddRouteComponents("odata", odataBuilder.GetEdmModel()));

            builder.Services.AddServices(builder.Configuration);

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
