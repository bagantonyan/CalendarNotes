using CalendarNotes.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CalendarNotes.DAL.Configurations
{
    internal class NoteConfiguration : BaseEntityConfiguration<Note>
    {
        public override void Configure(EntityTypeBuilder<Note> builder)
        {
            base.Configure(builder);

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title)
                .HasMaxLength(256)
                .IsRequired(true);

            builder.Property(p => p.Text)
                .HasMaxLength(10000)
                .IsRequired(true);

            builder.Property(p => p.NotificationTime)
                .IsRequired(true)
                .HasColumnType("timestamp without time zone"); // Используем timestamp без timezone

            builder.Property(p => p.IsNotified)
                .HasDefaultValue(false)
                .IsRequired(true);
        }
    }
}