# CalendarNotes

Система управления заметками с уведомлениями, аутентификацией и авторизацией.

## Архитектура проекта

Проект состоит из 6 основных компонентов:

- **CalendarNotes.API** - RESTful API для работы с заметками (порт 7135)
- **CalendarNotes.IdentityServer** - Сервер аутентификации с JWT (порт 7200)
- **CalendarNotes.Web** - 🆕 Современный фронтенд на Vue 3 + TypeScript + Pinia + Naive UI
- **CalendarNotes.BLL** - Бизнес-логика
- **CalendarNotes.DAL** - Слой доступа к данным (PostgreSQL)
- **CalendarNotes.Shared** - Общие компоненты

## Новые возможности ✨

### Аутентификация и авторизация
- ✅ JWT токены для безопасной аутентификации
- ✅ IdentityServer с ASP.NET Core Identity
- ✅ Регистрация и вход пользователей
- ✅ Защита всех API endpoints
- ✅ Автоматическое добавление токенов к HTTP запросам

### Исправленные проблемы
- ✅ Ошибка DateTime с PostgreSQL исправлена
- ✅ Поддержка timestamp without time zone

## Требования

- .NET 8.0 SDK
- PostgreSQL 12+
- Visual Studio 2022 или Rider (опционально)

## Настройка и запуск

### 1. Настройка базы данных

Обновите строки подключения в файлах `appsettings.json`:

**CalendarNotes.API/appsettings.json:**
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=CalendarNotesDB;Username=postgres;Password=ваш_пароль"
}
```

**CalendarNotes.IdentityServer/appsettings.json:**
```json
"ConnectionStrings": {
  "IdentityConnection": "Host=localhost;Port=5432;Database=CalendarNotesIdentityDB;Username=postgres;Password=ваш_пароль"
}
```

### 2. Применение миграций

База данных будет создана автоматически при первом запуске, но вы также можете применить миграции вручную:

```bash
# Для основной базы данных
cd CalendarNotes.API
dotnet ef database update --project ..\CalendarNotes.DAL\CalendarNotes.DAL.csproj

# Для базы данных Identity
cd CalendarNotes.IdentityServer
dotnet ef database update
```

### 3. Запуск приложений

Необходимо запустить **три сервиса**:

#### Запуск бэкенда (2 терминала):

```bash
# Терминал 1 - IdentityServer
cd CalendarNotes.IdentityServer
dotnet run

# Терминал 2 - API
cd CalendarNotes.API
dotnet run
```

#### Запуск фронтенда (Vue 3):

```bash
# Терминал 3 - Vue Frontend
cd CalendarNotes.Web
npm install  # Только при первом запуске
npm run dev
```

### 4. Доступ к приложениям

- **Vue Frontend:** http://localhost:5173
- **API Swagger:** https://localhost:7135/swagger
- **IdentityServer Swagger:** https://localhost:7200/swagger

## Использование

### Регистрация и вход

1. Откройте клиентское приложение
2. Нажмите "Регистрация" в правом верхнем углу
3. Заполните форму:
   - Email
   - Пароль (минимум 6 символов, 1 заглавная буква)
   - Имя и Фамилия
4. После регистрации вы будете автоматически авторизованы

### Тестовый администратор

При первом запуске автоматически создается администратор:
- **Email:** admin@calendarnotes.com
- **Пароль:** Admin123!

### Создание заметки

1. Войдите в систему
2. Перейдите в раздел "Заметки"
3. Нажмите "Создать новую заметку"
4. Заполните форму:
   - Заголовок
   - Текст
   - Время уведомления (локальное время)
5. Заметка будет сохранена

### Push-уведомления через SignalR

- Уведомления отправляются автоматически в указанное время
- SignalR Hub: `/notificationHub`
- Клиент автоматически подключается к hub (см. `Views/Home/Index.cshtml`)
- Поддерживает JWT аутентификацию для WebSocket соединений

## Технологический стек

### Backend
- ASP.NET Core 8.0 Web API
- Duende IdentityServer 7.x
- Entity Framework Core 9.0
- PostgreSQL (Npgsql)
- AutoMapper
- FluentValidation
- Serilog
- SignalR

### Frontend
- **Vue 3** - Composition API
- **TypeScript** - Типизация
- **Pinia** - State Management
- **Naive UI** - Component Library
- **Vue Router** - Маршрутизация
- **Axios** - HTTP клиент
- **SignalR** - Real-time уведомления
- **Vite** - Build tool

### Аутентификация
- JWT Bearer Tokens
- ASP.NET Core Identity
- Duende IdentityServer
- Password hashing (PBKDF2)

## Структура проекта

```
CalendarNotes/
├── CalendarNotes.API/           # REST API
│   ├── Controllers/             # API контроллеры
│   ├── Extensions/              # Расширения и конфигурация
│   ├── Filters/                 # Фильтры действий
│   ├── Handlers/                # Обработчики исключений
│   ├── Hubs/                    # SignalR hubs
│   └── Services/                # Background сервисы
├── CalendarNotes.IdentityServer/ # Сервер аутентификации
│   ├── Configuration/           # Конфигурация IdentityServer
│   ├── Controllers/             # Контроллеры аутентификации
│   ├── Data/                    # DbContext для Identity
│   ├── Models/                  # Модели пользователей
│   └── Services/                # Сервисы профилей
├── CalendarNotes.Client/        # Веб-клиент
│   ├── Controllers/             # MVC контроллеры
│   ├── Handlers/                # HTTP handlers
│   ├── Models/                  # ViewModels
│   ├── Services/                # Сервисы (AuthService)
│   └── Views/                   # Razor views
├── CalendarNotes.BLL/           # Бизнес-логика
│   ├── DTOs/                    # Data Transfer Objects
│   └── Services/                # Бизнес-сервисы
├── CalendarNotes.DAL/           # Доступ к данным
│   ├── Configurations/          # EF конфигурации
│   ├── Contexts/                # DbContext
│   ├── Entities/                # Модели данных
│   ├── Migrations/              # EF миграции
│   └── Repositories/            # Репозитории
└── CalendarNotes.Shared/        # Общие компоненты
    └── Exceptions/              # Кастомные исключения
```

## API Endpoints

Все endpoints требуют аутентификации (Bearer token).

### Заметки
- `GET /odata/Notes/GetAll` - Получить все заметки (с поддержкой OData)
- `GET /odata/Notes/GetById?noteId={id}` - Получить заметку по ID
- `POST /odata/Notes/Create` - Создать заметку
- `PUT /odata/Notes/Update` - Обновить заметку
- `DELETE /odata/Notes/Delete?noteId={id}` - Удалить заметку
- `GET /odata/Notes/ExportNotesToCsv` - Экспорт в CSV

### Аутентификация
- `POST /api/account/register` - Регистрация
- `POST /api/account/login` - Вход

## Конфигурация

### JWT Settings (appsettings.json)
```json
"Jwt": {
  "Key": "ваш_секретный_ключ_минимум_32_символа",
  "Issuer": "https://localhost:7200",
  "Audience": "calendarnotes.api"
}
```

### Настройки уведомлений
```json
"NoteServiceOptions": {
  "DelayTime": 30,              # Задержка в секундах
  "CheckIntervalMinutes": 1,     # Интервал проверки
  "UTCInterval": 3               # Разница с UTC (МСК = UTC+3)
}
```

## Особенности реализации

### DateTime и PostgreSQL
- Используется `timestamp without time zone`
- Включен `EnableLegacyTimestampBehavior` для Npgsql
- Все времена обрабатываются в локальном часовом поясе

### Безопасность
- Все пароли хэшируются с использованием PBKDF2
- JWT токены действительны 1 час
- HTTPS обязателен для production
- CORS настроен для разработки

### OData Support
API поддерживает OData запросы:
```
/odata/Notes/GetAll?$filter=IsNotified eq false&$orderby=NotificationTime desc&$top=10
```
## Лицензия

MIT

## Автор

Bagrat Antonyan
- Email: bag.antonyan@gmail.com
- GitHub: https://github.com/bagantonyan
