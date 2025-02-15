using AutoMapper;
using CalendarNotes.API.Models.Notes;
using CalendarNotes.BLL.DTOs.Notes;

namespace CalendarNotes.API.Mappings
{
    public class APIMappingProfile : Profile
    {
        public APIMappingProfile()
        {
            CreateMap<NoteResponseDTO, NoteResponseModel>();
            CreateMap<CreateNoteRequestModel, CreateNoteRequestDTO>();
            CreateMap<UpdateNoteRequestModel, UpdateNoteRequestDTO>();
        }
    }
}