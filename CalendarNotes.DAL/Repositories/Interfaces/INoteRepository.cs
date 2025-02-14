using CalendarNotes.DAL.Entities;

namespace CalendarNotes.DAL.Repositories.Interfaces
{
    public interface INoteRepository : IBaseRepository<Note>
    {
        Task<IEnumerable<Note>> GetAllAsync(bool trackChanges);
        Task<Note> GetByIdAsync(int eventId, bool trackChanges);
    }
}