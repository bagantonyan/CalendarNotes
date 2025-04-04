﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using CalendarNotes.BLL.DTOs.Notes;
using CalendarNotes.BLL.Services.Interfaces;
using CalendarNotes.DAL.Entities;
using CalendarNotes.DAL.UnitOfWork;
using CalendarNotes.Shared.Exceptions;

namespace CalendarNotes.BLL.Services
{
    public class NoteService : INoteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NoteService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<NoteResponseDTO> CreateAsync(CreateNoteRequestDTO requestDTO)
        {
            var noteEntity = _mapper.Map<Note>(requestDTO);

            await _unitOfWork.NoteRepository.CreateAsync(noteEntity);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<NoteResponseDTO>(noteEntity);
        }

        public async Task UpdateAsync(UpdateNoteRequestDTO requestDTO)
        {
            var noteEntity = await GetNoteEntityById(requestDTO.Id, trackChanges: true);

            _mapper.Map(requestDTO, noteEntity);

            _unitOfWork.NoteRepository.Update(noteEntity);

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int noteId)
        {
            var noteEntity = await GetNoteEntityById(noteId, trackChanges: true);

            _unitOfWork.NoteRepository.Delete(noteEntity);

            await _unitOfWork.SaveChangesAsync();
        }

        public IQueryable<NoteResponseDTO> GetAll(bool trackChanges)
        {
            var noteEntities = _unitOfWork.NoteRepository.GetAll(trackChanges);

            var noteResponseDTOs = noteEntities.ProjectTo<NoteResponseDTO>(_mapper.ConfigurationProvider);

            return noteResponseDTOs;
        }

        public async Task<NoteResponseDTO> GetByIdAsync(int noteId, bool trackChanges)
        {
            var noteEntity = await GetNoteEntityById(noteId, trackChanges);

            return _mapper.Map<NoteResponseDTO>(noteEntity);
        }

        private async Task<Note> GetNoteEntityById(int noteId, bool trackChanges)
        {
            var noteEntity = await _unitOfWork.NoteRepository.GetByIdAsync(noteId, trackChanges: true);

            if (noteEntity is null)
                throw new NoteNotFoundException(noteId);

            return noteEntity;
        }
    }
}