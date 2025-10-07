using AutoMapper;
using CalendarNotes.API.Models.Chat;
using CalendarNotes.BLL.DTOs.Chat;
using CalendarNotes.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CalendarNotes.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous] // Для демонстрации. В продакшене нужна авторизация
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IMapper _mapper;
        private readonly ILogger<ChatController> _logger;

        public ChatController(
            IChatService chatService,
            IMapper mapper,
            ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Создать новую комнату чата
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ChatRoomResponseModel>> CreateRoom(
            [FromBody] CreateChatRoomRequestModel requestModel,
            [FromQuery] string creatorUserId)
        {
            var dto = _mapper.Map<CreateChatRoomRequestDTO>(requestModel);
            var result = await _chatService.CreateRoomAsync(creatorUserId, dto);
            
            return CreatedAtAction(
                nameof(GetRoomById), 
                new { roomId = result.Id }, 
                _mapper.Map<ChatRoomResponseModel>(result));
        }

        /// <summary>
        /// Получить все комнаты пользователя
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ChatRoomResponseModel>>> GetUserRooms([FromQuery] string userId)
        {
            var rooms = await _chatService.GetUserRoomsAsync(userId);
            return Ok(_mapper.Map<IEnumerable<ChatRoomResponseModel>>(rooms));
        }

        /// <summary>
        /// Получить комнату по ID
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ChatRoomResponseModel>> GetRoomById([FromQuery] int roomId)
        {
            var room = await _chatService.GetRoomByIdAsync(roomId);
            return Ok(_mapper.Map<ChatRoomResponseModel>(room));
        }

        /// <summary>
        /// Получить сообщения комнаты
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ChatMessageResponseModel>>> GetRoomMessages(
            [FromQuery] int roomId,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 50)
        {
            var messages = await _chatService.GetRoomMessagesAsync(roomId, skip, take);
            return Ok(_mapper.Map<IEnumerable<ChatMessageResponseModel>>(messages));
        }

        /// <summary>
        /// Отправить сообщение (REST API альтернатива SignalR)
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ChatMessageResponseModel>> SendMessage(
            [FromBody] SendMessageRequestModel requestModel,
            [FromQuery] string senderId,
            [FromQuery] string senderName)
        {
            var dto = _mapper.Map<SendMessageRequestDTO>(requestModel);
            var result = await _chatService.SendMessageAsync(senderId, senderName, dto);
            
            return CreatedAtAction(
                nameof(GetRoomMessages), 
                new { roomId = result.ChatRoomId }, 
                _mapper.Map<ChatMessageResponseModel>(result));
        }

        /// <summary>
        /// Отметить сообщения комнаты как прочитанные
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MarkMessagesAsRead([FromQuery] int roomId)
        {
            await _chatService.MarkMessagesAsReadAsync(roomId);
            return Ok();
        }

        /// <summary>
        /// Получить или создать приватную комнату между двумя пользователями
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ChatRoomResponseModel>> GetOrCreatePrivateRoom(
            [FromQuery] string userId1,
            [FromQuery] string userId2)
        {
            var room = await _chatService.GetOrCreatePrivateRoomAsync(userId1, userId2);
            return Ok(_mapper.Map<ChatRoomResponseModel>(room));
        }
    }
}

