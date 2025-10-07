using Microsoft.AspNetCore.Identity;

namespace CalendarNotes.IdentityServer.Models
{
    /// <summary>
    /// Модель пользователя приложения
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string? FirstName { get; set; }
        
        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        public string? LastName { get; set; }
        
        /// <summary>
        /// Дата создания аккаунта
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        /// <summary>
        /// Полное имя пользователя
        /// </summary>
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
