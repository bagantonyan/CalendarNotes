using CalendarNotes.API.Extensions;
using CalendarNotes.BLL.Mappings;
using CalendarNotes.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CalendarNotes.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<CalendarNotesDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddServices(builder.Configuration);

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

            app.UseAuthorization();

            await app.MigrateDatabaseAsync();

            app.MapControllers();

            app.Run();
        }
    }
}
