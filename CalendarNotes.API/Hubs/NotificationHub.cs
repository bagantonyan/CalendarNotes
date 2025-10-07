using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CalendarNotes.API.Hubs
{
    /// <summary>
    /// SignalR Hub для отправки уведомлений клиентам
    /// </summary>
    [AllowAnonymous] // Разрешаем анонимный доступ для получения уведомлений
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
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
            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");
            if (exception != null)
            {
                _logger.LogError(exception, "Client disconnected with error");
            }
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Отправка уведомления всем подключенным клиентам
        /// </summary>
        public async Task SendNotification(string message)
        {
            _logger.LogInformation($"Broadcasting notification: {message}");
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}