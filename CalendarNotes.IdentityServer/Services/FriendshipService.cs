using CalendarNotes.IdentityServer.Data;
using CalendarNotes.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CalendarNotes.IdentityServer.Services
{
    public interface IFriendshipService
    {
        Task<Friendship> SendRequestAsync(string requesterId, string addresseeEmail);
        Task AcceptAsync(int friendshipId);
        Task RejectAsync(int friendshipId);
        Task<List<ApplicationUser>> GetFriendsAsync(string userId);
        Task<List<Friendship>> GetPendingAsync(string userId);
    }

    public class FriendshipService : IFriendshipService
    {
        private readonly IdentityDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public FriendshipService(IdentityDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<Friendship> SendRequestAsync(string requesterId, string addresseeEmail)
        {
            var addressee = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == addresseeEmail);
            if (addressee is null) throw new InvalidOperationException("Пользователь с таким email не найден");

            if (requesterId == addressee.Id) throw new InvalidOperationException("Нельзя добавить себя в друзья");

            var exists = await _db.Friendships.AnyAsync(f =>
                (f.RequesterId == requesterId && f.AddresseeId == addressee.Id) ||
                (f.RequesterId == addressee.Id && f.AddresseeId == requesterId));
            if (exists) throw new InvalidOperationException("Заявка в друзья уже существует");

            var friendship = new Friendship
            {
                RequesterId = requesterId,
                AddresseeId = addressee.Id,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };
            _db.Friendships.Add(friendship);
            await _db.SaveChangesAsync();
            return friendship;
        }

        public async Task AcceptAsync(int friendshipId)
        {
            var friendship = await _db.Friendships.FirstOrDefaultAsync(f => f.Id == friendshipId);
            if (friendship is null) throw new InvalidOperationException("Заявка не найдена");
            friendship.Status = "Accepted";
            friendship.RespondedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        public async Task RejectAsync(int friendshipId)
        {
            var friendship = await _db.Friendships.FirstOrDefaultAsync(f => f.Id == friendshipId);
            if (friendship is null) throw new InvalidOperationException("Заявка не найдена");
            friendship.Status = "Rejected";
            friendship.RespondedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        public async Task<List<ApplicationUser>> GetFriendsAsync(string userId)
        {
            // Друзья = все Accepted, где userId участвует как requester или addressee
            var friendIds = await _db.Friendships
                .Where(f => f.Status == "Accepted" && (f.RequesterId == userId || f.AddresseeId == userId))
                .Select(f => f.RequesterId == userId ? f.AddresseeId : f.RequesterId)
                .ToListAsync();

            return await _userManager.Users
                .Where(u => friendIds.Contains(u.Id))
                .ToListAsync();
        }

        public async Task<List<Friendship>> GetPendingAsync(string userId)
        {
            return await _db.Friendships
                .Where(f => f.Status == "Pending" && f.AddresseeId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }
    }
}

