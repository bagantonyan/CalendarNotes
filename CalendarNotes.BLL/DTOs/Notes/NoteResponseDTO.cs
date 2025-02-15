namespace CalendarNotes.BLL.DTOs.Notes
{
    public class NoteResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime NotificationTime { get; set; }
        public bool IsNotified { get; set; }
    }
}