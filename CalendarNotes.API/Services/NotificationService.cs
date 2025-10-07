using CalendarNotes.API.Hubs;
using CalendarNotes.DAL.UnitOfWork;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CalendarNotes.API.Services
{
    public class NotificationService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IServiceScopeFactory serviceScopeFactory,
            IHubContext<NotificationHub> hubContext,
            IConfiguration configuration,
            ILogger<NotificationService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _hubContext = hubContext;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancelToken)
        {
            _logger.LogInformation("NotificationService started");
            
            while (!cancelToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        // convert UtcNow to Now
                        var utcInterval = Convert.ToDouble(_configuration["NoteServiceOptions:UTCInterval"]);
                        var now = DateTime.UtcNow.AddHours(utcInterval);
                        var checkInterval = Convert.ToDouble(_configuration["NoteServiceOptions:CheckIntervalMinutes"]);

                        _logger.LogDebug($"Checking for notifications. Current time (UTC+{utcInterval}): {now}");

                        var notesToNotify = await unitOfWork.NoteRepository
                            .GetByCondition(n => !n.IsNotified 
                                               && n.NotificationTime <= now 
                                               && n.NotificationTime > now.AddMinutes(-checkInterval), 
                                               trackChanges: true).ToListAsync(cancelToken);

                        if (notesToNotify.Any())
                        {
                            _logger.LogInformation($"Found {notesToNotify.Count} notes to notify");
                        }

                        foreach (var note in notesToNotify)
                        {
                            var message = $"Notification: {note.Title} - {note.Text}";

                            _logger.LogInformation($"Sending notification for note ID {note.Id}: {message}");
                            
                            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message, cancellationToken: cancelToken);

                            note.IsNotified = true;
                            
                            _logger.LogInformation($"Successfully sent notification for note ID {note.Id}");
                        }

                        if (notesToNotify.Any())
                        {
                            await unitOfWork.SaveChangesAsync();
                            _logger.LogInformation($"Marked {notesToNotify.Count} notes as notified");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in NotificationService");
                }

                var delayTime = TimeSpan.FromSeconds(Convert.ToDouble(_configuration["NoteServiceOptions:DelayTime"]));
                await Task.Delay(delayTime, cancelToken);
            }
            
            _logger.LogInformation("NotificationService stopped");
        }
    }
}