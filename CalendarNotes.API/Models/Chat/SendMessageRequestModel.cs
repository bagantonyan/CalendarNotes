using System.ComponentModel.DataAnnotations;

namespace CalendarNotes.API.Models.Chat
{
    /// <summary>
    /// Модель запроса для отправки сообщения
    /// </summary>
    public class SendMessageRequestModel
    {
        [Required(ErrorMessage = "ID комнаты обязателен")]
        public int ChatRoomId { get; set; }

        [Required(ErrorMessage = "Содержимое сообщения обязательно")]
        [MaxLength(5000, ErrorMessage = "Сообщение не может быть длиннее 5000 символов")]
        public string Content { get; set; } = string.Empty;
    }
}

