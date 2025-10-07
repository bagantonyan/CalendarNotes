using CalendarNotes.IdentityServer.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CalendarNotes.IdentityServer.Data
{
    /// <summary>
    /// Контекст базы данных для Identity
    /// </summary>
    public class IdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {
        }

        public DbSet<Friendship> Friendships { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Настройка схемы базы данных
            builder.HasDefaultSchema("identity");
            
            // Дополнительные настройки для моделей
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
            });

            builder.Entity<Friendship>(entity =>
            {
                entity.ToTable("Friendships");
                entity.HasKey(f => f.Id);
                entity.Property(f => f.RequesterId).HasMaxLength(256).IsRequired();
                entity.Property(f => f.AddresseeId).HasMaxLength(256).IsRequired();
                entity.Property(f => f.Status).HasMaxLength(32).IsRequired();
                entity.HasIndex(f => new { f.RequesterId, f.AddresseeId }).IsUnique();
            });
        }
    }
}
