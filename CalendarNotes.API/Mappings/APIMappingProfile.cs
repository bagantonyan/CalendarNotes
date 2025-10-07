using AutoMapper;
using CalendarNotes.API.Models.Notes;
using CalendarNotes.API.Models.Chat;
using CalendarNotes.BLL.DTOs.Notes;
using CalendarNotes.BLL.DTOs.Chat;

namespace CalendarNotes.API.Mappings
{
    public class APIMappingProfile : Profile
    {
        public APIMappingProfile()
        {
            // Маппинг для заметок
            CreateMap<NoteResponseDTO, NoteResponseModel>();
            CreateMap<CreateNoteRequestModel, CreateNoteRequestDTO>();
            CreateMap<UpdateNoteRequestModel, UpdateNoteRequestDTO>();

            // Маппинг для чата
            CreateMap<ChatRoomResponseDTO, ChatRoomResponseModel>();
            CreateMap<ChatMessageResponseDTO, ChatMessageResponseModel>();
            CreateMap<CreateChatRoomRequestModel, CreateChatRoomRequestDTO>();
            CreateMap<SendMessageRequestModel, SendMessageRequestDTO>();
        }
    }
}