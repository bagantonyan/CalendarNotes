using System.ComponentModel.DataAnnotations;

namespace CalendarNotes.API.Models.Chat
{
    /// <summary>
    /// Модель запроса для создания комнаты чата
    /// </summary>
    public class CreateChatRoomRequestModel
    {
        [MaxLength(256)]
        public string? Name { get; set; }

        public bool IsGroupChat { get; set; }

        [Required(ErrorMessage = "Список участников обязателен")]
        [MinLength(1, ErrorMessage = "Должен быть хотя бы один участник")]
        public List<string> ParticipantIds { get; set; } = new();
    }
}

