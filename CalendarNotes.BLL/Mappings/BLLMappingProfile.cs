using AutoMapper;
using CalendarNotes.BLL.DTOs.Notes;
using CalendarNotes.BLL.DTOs.Chat;
using CalendarNotes.DAL.Entities;

namespace CalendarNotes.BLL.Mappings
{
    public class BLLMappingProfile : Profile
    {
        public BLLMappingProfile()
        {
            // Маппинг для заметок
            CreateMap<CreateNoteRequestDTO, Note>();
            CreateMap<Note, NoteResponseDTO>().ReverseMap();
            CreateMap<UpdateNoteRequestDTO, Note>().ReverseMap();

            // Маппинг для чата
            CreateMap<ChatMessage, ChatMessageResponseDTO>();
            CreateMap<ChatRoom, ChatRoomResponseDTO>();
        }
    }
}