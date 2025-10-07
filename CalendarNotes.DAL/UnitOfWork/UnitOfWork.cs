using CalendarNotes.DAL.Contexts;
using CalendarNotes.DAL.Repositories;
using CalendarNotes.DAL.Repositories.Interfaces;

namespace CalendarNotes.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CalendarNotesDbContext _dbContext;

        public UnitOfWork(CalendarNotesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private readonly INoteRepository _noteRepository;
        public INoteRepository NoteRepository =>
            _noteRepository is not null ? _noteRepository : new NoteRepository(_dbContext);

        private readonly IChatRoomRepository _chatRoomRepository;
        public IChatRoomRepository ChatRoomRepository =>
            _chatRoomRepository is not null ? _chatRoomRepository : new ChatRoomRepository(_dbContext);

        private readonly IChatMessageRepository _chatMessageRepository;
        public IChatMessageRepository ChatMessageRepository =>
            _chatMessageRepository is not null ? _chatMessageRepository : new ChatMessageRepository(_dbContext);

        public async Task SaveChangesAsync() => await _dbContext.SaveChangesAsync();
        public void Dispose() => _dbContext.Dispose();
    }
}