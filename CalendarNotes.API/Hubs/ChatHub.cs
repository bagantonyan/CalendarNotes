using CalendarNotes.BLL.DTOs.Chat;
using CalendarNotes.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace CalendarNotes.API.Hubs
{
    /// <summary>
    /// SignalR Hub для чата в реальном времени
    /// </summary>
    [AllowAnonymous] // Для демонстрации. В продакшене нужна авторизация
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatHub> _logger;
        
        // Хранение соответствия пользователей и их connection ID
        private static readonly ConcurrentDictionary<string, HashSet<string>> _userConnections = new();

        public ChatHub(IChatService chatService, ILogger<ChatHub> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        /// <summary>
        /// Регистрация пользователя при подключении
        /// </summary>
        public async Task RegisterUser(string userId, string userName)
        {
            _userConnections.AddOrUpdate(
                userId,
                new HashSet<string> { Context.ConnectionId },
                (key, existing) =>
                {
                    existing.Add(Context.ConnectionId);
                    return existing;
                });

            _logger.LogInformation($"User {userName} ({userId}) connected: {Context.ConnectionId}");
            
            // Уведомляем пользователя о успешной регистрации
            await Clients.Caller.SendAsync("UserRegistered", userId);
        }

        /// <summary>
        /// Присоединение к комнате чата
        /// </summary>
        public async Task JoinRoom(int roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GetRoomGroupName(roomId));
            _logger.LogInformation($"Connection {Context.ConnectionId} joined room {roomId}");
            
            // Уведомляем пользователя о присоединении к комнате
            await Clients.Caller.SendAsync("JoinedRoom", roomId);
        }

        /// <summary>
        /// Выход из комнаты чата
        /// </summary>
        public async Task LeaveRoom(int roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetRoomGroupName(roomId));
            _logger.LogInformation($"Connection {Context.ConnectionId} left room {roomId}");
        }

        /// <summary>
        /// Отправка сообщения в комнату
        /// </summary>
        public async Task SendMessage(string senderId, string senderName, int roomId, string content)
        {
            try
            {
                var messageRequest = new SendMessageRequestDTO
                {
                    ChatRoomId = roomId,
                    Content = content
                };

                var message = await _chatService.SendMessageAsync(senderId, senderName, messageRequest);

                // Отправляем сообщение всем участникам комнаты
                await Clients.Group(GetRoomGroupName(roomId)).SendAsync("ReceiveMessage", message);
                
                _logger.LogInformation($"Message sent to room {roomId} by {senderName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending message to room {roomId}");
                await Clients.Caller.SendAsync("Error", "Не удалось отправить сообщение");
            }
        }

        /// <summary>
        /// Уведомление о том, что пользователь печатает
        /// </summary>
        public async Task UserTyping(int roomId, string userName)
        {
            await Clients.OthersInGroup(GetRoomGroupName(roomId)).SendAsync("UserTyping", roomId, userName);
        }

        /// <summary>
        /// Уведомление о том, что пользователь перестал печатать
        /// </summary>
        public async Task UserStoppedTyping(int roomId, string userName)
        {
            await Clients.OthersInGroup(GetRoomGroupName(roomId)).SendAsync("UserStoppedTyping", roomId, userName);
        }

        /// <summary>
        /// Отметить сообщения как прочитанные
        /// </summary>
        public async Task MarkAsRead(int roomId)
        {
            try
            {
                await _chatService.MarkMessagesAsReadAsync(roomId);
                
                // Уведомляем других участников, что сообщения прочитаны
                await Clients.OthersInGroup(GetRoomGroupName(roomId)).SendAsync("MessagesRead", roomId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error marking messages as read in room {roomId}");
            }
        }

        /// <summary>
        /// Вызывается при подключении клиента
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Вызывается при отключении клиента
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Удаляем все записи о подключении этого пользователя
            foreach (var kvp in _userConnections)
            {
                kvp.Value.Remove(Context.ConnectionId);
                if (kvp.Value.Count == 0)
                {
                    _userConnections.TryRemove(kvp.Key, out _);
                }
            }

            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");
            if (exception != null)
            {
                _logger.LogError(exception, "Client disconnected with error");
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Получить имя группы для комнаты
        /// </summary>
        private static string GetRoomGroupName(int roomId) => $"ChatRoom_{roomId}";

        /// <summary>
        /// Отправить сообщение конкретному пользователю
        /// </summary>
        public async Task SendDirectMessage(string targetUserId, object message)
        {
            if (_userConnections.TryGetValue(targetUserId, out var connectionIds))
            {
                await Clients.Clients(connectionIds.ToList()).SendAsync("ReceiveDirectMessage", message);
            }
        }
    }
}

