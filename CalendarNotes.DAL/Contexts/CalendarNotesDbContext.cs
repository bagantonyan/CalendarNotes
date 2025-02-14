using CalendarNotes.DAL.Configurations;
using CalendarNotes.DAL.Entities;
using CalendarNotes.DAL.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CalendarNotes.DAL.Contexts
{
    public class CalendarNotesDbContext(DbContextOptions<CalendarNotesDbContext> options) : DbContext(options)
    {
        public DbSet<Note> Notes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new NoteConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.SetAuditProperties();

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}