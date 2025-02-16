# CalendarNotes

1) Change postgres username and password to your's in CalendarNotes.API --> appsettings.json --> ConnectionStrings --> DefaultConnection
2) The database will be automaticly created when running the application
3) Run CalendarNotes.API and CalendarNotes.Client together (set multiple startup projects)
4) When creating a Note please set NotificationTime to local Moscow time
5) Push Notifications has been implemented with SignalR. CalendarNotes.API sends notification and CalendarNotes.Client receives it by SignalR
