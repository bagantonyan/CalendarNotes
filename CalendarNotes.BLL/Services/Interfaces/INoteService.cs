using CalendarNotes.BLL.DTOs.Notes;

namespace CalendarNotes.BLL.Services.Interfaces
{
    public interface INoteService
    {
        Task<NoteResponseDTO> CreateAsync(CreateNoteRequestDTO requestDTO);
        Task UpdateAsync(UpdateNoteRequestDTO requestDTO);
        Task DeleteAsync(int noteId);
        IQueryable<NoteResponseDTO> GetAll(bool trackChanges);
        Task<NoteResponseDTO> GetByIdAsync(int noteId, bool trackChanges);
    }
}