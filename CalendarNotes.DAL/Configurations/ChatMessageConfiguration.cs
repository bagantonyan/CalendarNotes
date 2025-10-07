using CalendarNotes.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CalendarNotes.DAL.Configurations
{
    internal class ChatMessageConfiguration : BaseEntityConfiguration<ChatMessage>
    {
        public override void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            base.Configure(builder);

            builder.HasKey(p => p.Id);

            builder.Property(p => p.ChatRoomId)
                .IsRequired(true);

            builder.Property(p => p.SenderId)
                .HasMaxLength(256)
                .IsRequired(true);

            builder.Property(p => p.SenderName)
                .HasMaxLength(256)
                .IsRequired(true);

            builder.Property(p => p.Content)
                .HasMaxLength(5000)
                .IsRequired(true);

            builder.Property(p => p.IsRead)
                .HasDefaultValue(false)
                .IsRequired(true);

            // Индекс для быстрого поиска сообщений по комнате
            builder.HasIndex(p => p.ChatRoomId);
            
            // Индекс для поиска непрочитанных сообщений
            builder.HasIndex(p => new { p.ChatRoomId, p.IsRead });
        }
    }
}

