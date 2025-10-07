using CalendarNotes.IdentityServer.Models;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CalendarNotes.IdentityServer.Services
{
    /// <summary>
    /// Сервис профилей пользователей для IdentityServer
    /// </summary>
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Получение данных профиля пользователя
        /// </summary>
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim("sub", user.Id),
                    new Claim("name", user.FullName),
                    new Claim("email", user.Email ?? string.Empty),
                    new Claim("given_name", user.FirstName ?? string.Empty),
                    new Claim("family_name", user.LastName ?? string.Empty)
                };

                // Добавляем роли пользователя
                var roles = await _userManager.GetRolesAsync(user);
                claims.AddRange(roles.Select(role => new Claim("role", role)));

                context.IssuedClaims = claims;
            }
        }

        /// <summary>
        /// Проверка активности пользователя
        /// </summary>
        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            context.IsActive = user != null;
        }
    }
}
