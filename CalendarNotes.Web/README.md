# CalendarNotes.Web - Vue 3 Frontend

Современный фронтенд для системы управления заметками CalendarNotes, построенный на Vue 3, TypeScript, Pinia и Naive UI.

## Технологии

- **Vue 3** - Progressive JavaScript Framework
- **TypeScript** - Типизированный JavaScript
- **Vite** - Быстрый сборщик
- **Vue Router** - Маршрутизация
- **Pinia** - Управление состоянием
- **Naive UI** - Компонентная библиотека
- **Axios** - HTTP клиент
- **SignalR** - Реальное время уведомления

## Установка

```bash
npm install
```

## Запуск в режиме разработки

```bash
npm run dev
```

Приложение будет доступно на `http://localhost:5173`

## Сборка для продакшена

```bash
npm run build
```

## Preview продакшен сборки

```bash
npm run preview
```

## Структура проекта

```
src/
├── components/      # Переиспользуемые компоненты
│   ├── AppHeader.vue
│   └── NotificationsPanel.vue
├── views/           # Страницы приложения
│   ├── Home.vue
│   ├── Login.vue
│   ├── Register.vue
│   ├── Notes.vue
│   ├── CreateNote.vue
│   ├── EditNote.vue
│   └── NotFound.vue
├── stores/          # Pinia stores
│   ├── auth.ts
│   ├── notes.ts
│   └── notifications.ts
├── services/        # API сервисы
│   ├── api.ts
│   └── signalr.ts
├── router/          # Vue Router конфигурация
│   └── index.ts
├── types/           # TypeScript типы
│   └── index.ts
├── App.vue          # Корневой компонент
└── main.ts          # Точка входа

```

## Конфигурация

Настройте URL бэкенда в файле `.env`:

```env
VITE_API_URL=https://localhost:7135
VITE_IDENTITY_URL=https://localhost:7200
```

## Особенности

### Аутентификация
- JWT токены для безопасного доступа
- Автоматическое добавление токена к запросам
- Защищенные маршруты
- Перенаправление на login при истечении токена

### SignalR Уведомления
- Автоматическое подключение при загрузке
- Визуальный индикатор состояния подключения
- Всплывающие уведомления на странице
- Браузерные push-уведомления
- Автоматическое переподключение

### Управление заметками
- Создание, редактирование, удаление
- Установка времени уведомления
- Экспорт в CSV
- Статус уведомлений

## Требования

- Node.js 20.19.0+ или 22.12.0+
- npm 10+

## API Endpoints

Приложение работает с следующими API:

- **IdentityServer:** `https://localhost:7200`
  - POST `/api/account/login` - Вход
  - POST `/api/account/register` - Регистрация

- **API:** `https://localhost:7135`
  - GET `/odata/Notes/GetAll` - Список заметок
  - GET `/odata/Notes/GetById` - Получить заметку
  - POST `/odata/Notes/Create` - Создать заметку
  - PUT `/odata/Notes/Update` - Обновить заметку
  - DELETE `/odata/Notes/Delete` - Удалить заметку
  - GET `/odata/Notes/ExportNotesToCsv` - Экспорт

- **SignalR Hub:** `https://localhost:7135/notificationHub`

## Тестовый пользователь

Email: `admin@calendarnotes.com`
Пароль: `Admin123!`

