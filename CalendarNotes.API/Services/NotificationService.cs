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

        public NotificationService(
            IServiceScopeFactory serviceScopeFactory,
            IHubContext<NotificationHub> hubContext,
            IConfiguration configuration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _hubContext = hubContext;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken cancelToken)
        {
            while (!cancelToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    // convert UtcNow to Now
                    var now = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["NoteServiceOptions:UTCInterval"]));

                    var notesToNotify = await unitOfWork.NoteRepository
                        .GetByCondition(n => !n.IsNotified 
                                           && n.NotificationTime <= now 
                                           && n.NotificationTime > now.AddMinutes(-Convert.ToDouble(_configuration["NoteServiceOptions:CheckIntervalMinutes"])), 
                                           trackChanges: true).ToListAsync(cancelToken);

                    foreach (var note in notesToNotify)
                    {
                        var message = $"Notification: {note.Title} - {note.Text}";

                        await _hubContext.Clients.All.SendAsync("ReceiveNotification", message, cancellationToken: cancelToken);

                        note.IsNotified = true;
                    }

                    await unitOfWork.SaveChangesAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(Convert.ToDouble(_configuration["NoteServiceOptions:DelayTime"])), cancelToken);
            }
        }
    }
}