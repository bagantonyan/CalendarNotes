namespace CalendarNotes.API.Models.Notes
{
    public class UpdateNoteRequestModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime NotificationTime { get; set; }
    }
}