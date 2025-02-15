using CalendarNotes.DAL.Entities;

namespace CalendarNotes.DAL.Repositories.Interfaces
{
    public interface INoteRepository : IBaseRepository<Note>
    {
        IQueryable<Note> GetAllAsync(bool trackChanges);
        Task<Note> GetByIdAsync(int eventId, bool trackChanges);
    }
}