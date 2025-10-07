using CalendarNotes.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CalendarNotes.DAL.Configurations
{
    internal class ChatRoomConfiguration : BaseEntityConfiguration<ChatRoom>
    {
        public override void Configure(EntityTypeBuilder<ChatRoom> builder)
        {
            base.Configure(builder);

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .HasMaxLength(256)
                .IsRequired(false);

            builder.Property(p => p.IsGroupChat)
                .HasDefaultValue(false)
                .IsRequired(true);

            builder.Property(p => p.CreatorUserId)
                .HasMaxLength(256)
                .IsRequired(true);

            builder.Property(p => p.ParticipantIds)
                .HasMaxLength(2000)
                .IsRequired(true);

            // Связь один ко многим с сообщениями
            builder.HasMany(p => p.Messages)
                .WithOne(m => m.ChatRoom)
                .HasForeignKey(m => m.ChatRoomId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

