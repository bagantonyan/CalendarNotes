using CalendarNotes.DAL.Contexts;
using CalendarNotes.DAL.Entities;
using CalendarNotes.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CalendarNotes.DAL.Repositories
{
    public class ChatRoomRepository(CalendarNotesDbContext dbContext) 
        : BaseRepository<ChatRoom>(dbContext), IChatRoomRepository
    {
        public async Task<ChatRoom?> GetByIdAsync(int roomId, bool trackChanges)
            => await GetByCondition(r => r.Id == roomId, trackChanges)
                .Include(r => r.Messages)
                .SingleOrDefaultAsync();

        public async Task<List<ChatRoom>> GetUserRoomsAsync(string userId, bool trackChanges)
            => await GetByCondition(r => r.ParticipantIds.Contains(userId), trackChanges)
                .Include(r => r.Messages.OrderByDescending(m => m.CreatedDate).Take(1))
                .OrderByDescending(r => r.ModifiedDate)
                .ToListAsync();

        public async Task<ChatRoom?> FindPrivateRoomAsync(string userId1, string userId2, bool trackChanges)
            => await GetByCondition(
                r => !r.IsGroupChat && 
                     r.ParticipantIds.Contains(userId1) && 
                     r.ParticipantIds.Contains(userId2), 
                trackChanges)
                .FirstOrDefaultAsync();
    }
}

