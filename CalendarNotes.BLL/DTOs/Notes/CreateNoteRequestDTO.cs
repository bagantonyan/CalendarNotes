namespace CalendarNotes.BLL.DTOs.Notes
{
    public class CreateNoteRequestDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime NotificationTime { get; set; }
    }
}