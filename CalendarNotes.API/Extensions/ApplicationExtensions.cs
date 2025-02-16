using CalendarNotes.DAL.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CalendarNotes.API.Extensions
{
    public static class ApplicationExtensions
    {
        public static async Task<WebApplication> MigrateDatabaseAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                using (var dbContext = scope.ServiceProvider.GetRequiredService<CalendarNotesDbContext>())
                {
                    await dbContext.Database.MigrateAsync();
                }
            }

            return app;
        }

        public static void AddSwagger(this WebApplication app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(configs =>
            {
                configs.SwaggerEndpoint("/swagger/v1/swagger.json", "Note API V1");
            });
        }
    }
}