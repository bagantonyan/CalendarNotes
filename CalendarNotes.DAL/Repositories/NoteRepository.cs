using CalendarNotes.DAL.Contexts;
using CalendarNotes.DAL.Entities;
using CalendarNotes.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CalendarNotes.DAL.Repositories
{
    public class NoteRepository(CalendarNotesDbContext dbContext) : BaseRepository<Note>(dbContext), INoteRepository
    {
        public IQueryable<Note> GetAllAsync(bool trackChanges)
            => GetAll(trackChanges);

        public async Task<Note> GetByIdAsync(int eventId, bool trackChanges)
            => await GetByCondition(e => e.Id == eventId, trackChanges)
                    .SingleOrDefaultAsync();
    }
}