namespace CalendarNotes.IdentityServer.Models
{
    /// <summary>
    /// Дружба между пользователями
    /// </summary>
    public class Friendship
    {
        public int Id { get; set; }

        /// <summary>
        /// Инициатор дружбы
        /// </summary>
        public string RequesterId { get; set; } = string.Empty;

        /// <summary>
        /// Адресат дружбы
        /// </summary>
        public string AddresseeId { get; set; } = string.Empty;

        /// <summary>
        /// Статус дружбы: Pending | Accepted | Rejected
        /// </summary>
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RespondedAt { get; set; }
    }
}

