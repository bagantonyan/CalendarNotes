using AutoMapper;
using CalendarNotes.BLL.DTOs.Chat;
using CalendarNotes.BLL.Services.Interfaces;
using CalendarNotes.DAL.Entities;
using CalendarNotes.DAL.UnitOfWork;
using CalendarNotes.Shared.Exceptions;

namespace CalendarNotes.BLL.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ChatService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ChatRoomResponseDTO> CreateRoomAsync(string creatorUserId, CreateChatRoomRequestDTO requestDTO)
        {
            var participantIds = new List<string>(requestDTO.ParticipantIds);
            
            // Добавляем создателя в список участников, если его там нет
            if (!participantIds.Contains(creatorUserId))
            {
                participantIds.Add(creatorUserId);
            }

            var chatRoom = new ChatRoom
            {
                Name = requestDTO.Name,
                IsGroupChat = requestDTO.IsGroupChat,
                CreatorUserId = creatorUserId,
                ParticipantIds = string.Join(",", participantIds)
            };

            await _unitOfWork.ChatRoomRepository.CreateAsync(chatRoom);
            await _unitOfWork.SaveChangesAsync();

            return MapToResponseDTO(chatRoom, 0);
        }

        public async Task<List<ChatRoomResponseDTO>> GetUserRoomsAsync(string userId)
        {
            var rooms = await _unitOfWork.ChatRoomRepository.GetUserRoomsAsync(userId, trackChanges: false);

            var roomDTOs = new List<ChatRoomResponseDTO>();
            foreach (var room in rooms)
            {
                var unreadCount = await _unitOfWork.ChatMessageRepository.GetUnreadCountAsync(room.Id);
                roomDTOs.Add(MapToResponseDTO(room, unreadCount));
            }

            return roomDTOs;
        }

        public async Task<ChatRoomResponseDTO> GetRoomByIdAsync(int roomId)
        {
            var room = await _unitOfWork.ChatRoomRepository.GetByIdAsync(roomId, trackChanges: false);
            
            if (room is null)
                throw new ChatRoomNotFoundException(roomId);

            var unreadCount = await _unitOfWork.ChatMessageRepository.GetUnreadCountAsync(roomId);
            
            return MapToResponseDTO(room, unreadCount);
        }

        public async Task<ChatMessageResponseDTO> SendMessageAsync(string senderId, string senderName, SendMessageRequestDTO requestDTO)
        {
            var room = await _unitOfWork.ChatRoomRepository.GetByIdAsync(requestDTO.ChatRoomId, trackChanges: false);
            
            if (room is null)
                throw new ChatRoomNotFoundException(requestDTO.ChatRoomId);

            var message = new ChatMessage
            {
                ChatRoomId = requestDTO.ChatRoomId,
                SenderId = senderId,
                SenderName = senderName,
                Content = requestDTO.Content,
                IsRead = false
            };

            await _unitOfWork.ChatMessageRepository.CreateAsync(message);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ChatMessageResponseDTO>(message);
        }

        public async Task<List<ChatMessageResponseDTO>> GetRoomMessagesAsync(int roomId, int skip = 0, int take = 50)
        {
            var messages = await _unitOfWork.ChatMessageRepository.GetRoomMessagesAsync(
                roomId, skip, take, trackChanges: false);

            return _mapper.Map<List<ChatMessageResponseDTO>>(messages);
        }

        public async Task MarkMessagesAsReadAsync(int roomId)
        {
            await _unitOfWork.ChatMessageRepository.MarkAllAsReadAsync(roomId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ChatRoomResponseDTO> GetOrCreatePrivateRoomAsync(string userId1, string userId2)
        {
            // Проверяем, существует ли уже приватная комната между пользователями
            var existingRoom = await _unitOfWork.ChatRoomRepository.FindPrivateRoomAsync(
                userId1, userId2, trackChanges: false);

            if (existingRoom != null)
            {
                var unreadCount = await _unitOfWork.ChatMessageRepository.GetUnreadCountAsync(existingRoom.Id);
                return MapToResponseDTO(existingRoom, unreadCount);
            }

            // Создаем новую приватную комнату
            var createRequest = new CreateChatRoomRequestDTO
            {
                IsGroupChat = false,
                ParticipantIds = new List<string> { userId1, userId2 }
            };

            return await CreateRoomAsync(userId1, createRequest);
        }

        private ChatRoomResponseDTO MapToResponseDTO(ChatRoom room, int unreadCount)
        {
            var dto = new ChatRoomResponseDTO
            {
                Id = room.Id,
                Name = room.Name,
                IsGroupChat = room.IsGroupChat,
                CreatorUserId = room.CreatorUserId,
                ParticipantIds = room.ParticipantIds.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                CreatedDate = room.CreatedDate,
                ModifiedDate = room.ModifiedDate,
                UnreadCount = unreadCount
            };

            // Добавляем последнее сообщение, если оно есть
            if (room.Messages != null && room.Messages.Any())
            {
                var lastMessage = room.Messages.OrderByDescending(m => m.CreatedDate).First();
                dto.LastMessage = _mapper.Map<ChatMessageResponseDTO>(lastMessage);
            }

            return dto;
        }
    }
}

