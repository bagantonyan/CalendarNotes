﻿namespace CalendarNotes.DAL.Entities
{
    public class Note : BaseEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime NoteTime { get; set; }
        public bool IsNotified { get; set; }
    }
}