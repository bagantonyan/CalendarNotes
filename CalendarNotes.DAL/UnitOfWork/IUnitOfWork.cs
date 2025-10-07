using CalendarNotes.DAL.Repositories.Interfaces;

namespace CalendarNotes.DAL.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        INoteRepository NoteRepository { get; }
        IChatRoomRepository ChatRoomRepository { get; }
        IChatMessageRepository ChatMessageRepository { get; }
        Task SaveChangesAsync();
    }
}