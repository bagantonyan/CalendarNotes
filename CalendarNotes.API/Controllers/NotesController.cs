using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalendarNotes.BLL.Services.Interfaces;
using CalendarNotes.API.Models.Notes;
using Microsoft.AspNetCore.OData.Query;
using AutoMapper;
using CalendarNotes.BLL.DTOs.Notes;
using System.Text;

namespace CalendarNotes.API.Controllers
{
    [Route("odata/[controller]/[action]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly INoteService _noteService;
        private readonly IMapper _mapper;

        public NotesController(
            INoteService noteService,
            IMapper mapper)
        {
            _noteService = noteService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get notes with OData queries
        /// </summary>
        /// <returns>List of notes</returns>
        [HttpGet]
        [EnableQuery]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<NoteResponseModel>>> GetAll()
        {
            var noteResponseDtos = _noteService.GetAll(trackChanges: false);

            var noteResponseDtosResult = await noteResponseDtos.ToListAsync();

            return Ok(_mapper.Map<IEnumerable<NoteResponseModel>>(noteResponseDtosResult));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<NoteResponseModel>> GetById(int noteId)
        {
            var noteResponseDTO = await _noteService.GetByIdAsync(noteId, trackChanges: false);

            return Ok(_mapper.Map<NoteResponseModel>(noteResponseDTO));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<NoteResponseModel>> Create(CreateNoteRequestModel requestModel)
        {
            var noteResponseDTO = await _noteService.CreateAsync(_mapper.Map<CreateNoteRequestDTO>(requestModel));

            return CreatedAtAction(nameof(GetById), new { noteId = noteResponseDTO.Id }, _mapper.Map<NoteResponseModel>(noteResponseDTO));
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(UpdateNoteRequestModel requestModel)
        {
            await _noteService.UpdateAsync(_mapper.Map<UpdateNoteRequestDTO>(requestModel));

            return Ok();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int noteId)
        {
            await _noteService.DeleteAsync(noteId);

            return Ok();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportNotesToCsv()
        {
            var noteResponseDtos = await _noteService.GetAll(trackChanges: false).ToListAsync();

            var csvBuilder = new StringBuilder();

            csvBuilder.AppendLine("Id,Title,Text,NotificationTime,CreatedDate,IsNotified");

            foreach (var note in noteResponseDtos)
            {
                csvBuilder.AppendLine($"{note.Id},{note.Title},{note.Text},{note.NotificationTime},{note.IsNotified}");
            }

            return File(Encoding.UTF8.GetBytes(csvBuilder.ToString()), "text/csv", "notes.csv");
        }
    }
}