using AutoMapper;
using CalendarNotes.BLL.DTOs.Notes;
using CalendarNotes.DAL.Entities;

namespace CalendarNotes.BLL.Mappings
{
    public class BLLMappingProfile : Profile
    {
        public BLLMappingProfile()
        {
            CreateMap<CreateNoteRequestDTO, Note>();
            CreateMap<Note, NoteResponseDTO>().ReverseMap();
        }
    }
}