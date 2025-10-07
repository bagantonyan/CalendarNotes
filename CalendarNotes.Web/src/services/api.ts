import axios, { type AxiosInstance } from 'axios'
import type {
  LoginRequest,
  RegisterRequest,
  AuthResponse,
  Note,
  CreateNoteRequest,
  UpdateNoteRequest,
  ChatRoom,
  ChatMessage,
  CreateChatRoomRequest,
  SendMessageRequest
} from '@/types'

// Конфигурация API
const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:7135'
const IDENTITY_BASE_URL = import.meta.env.VITE_IDENTITY_URL || 'https://localhost:7200'

// Создаем экземпляр axios для API
const apiClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Создаем экземпляр axios для IdentityServer
const identityClient: AxiosInstance = axios.create({
  baseURL: IDENTITY_BASE_URL,
  headers: {
    'Content-Type': 'application/json'
  }
})

// Интерцептор для добавления токена к запросам IdentityServer
identityClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('auth_token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => Promise.reject(error)
)

// Интерцептор для обработки 401 и других ошибок IdentityServer
identityClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('auth_token')
      localStorage.removeItem('user')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

// Интерцептор для добавления токена к запросам API
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('auth_token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => Promise.reject(error)
)

// Интерцептор для обработки ошибок
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Токен истек или невалиден
      localStorage.removeItem('auth_token')
      localStorage.removeItem('user')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

// API методы для аутентификации
export const authApi = {
  async login(data: LoginRequest): Promise<AuthResponse> {
    const response = await identityClient.post<AuthResponse>('/api/account/login', data)
    return response.data
  },

  async register(data: RegisterRequest): Promise<{ message: string }> {
    const response = await identityClient.post<{ message: string }>(
      '/api/account/register',
      data
    )
    return response.data
  }
}

// Интерфейс для обертки API ответа
interface ApiResponseWrapper<T> {
  data?: T[]
  succeeded?: boolean
  message?: string | null
  statusCode?: number
}

// Интерфейс для OData ответа
interface ODataResponse<T> {
  value?: T[]
  '@odata.context'?: string
}

// API методы для заметок
export const notesApi = {
  async getAll(): Promise<Note[]> {
    const response = await apiClient.get<Note[] | ApiResponseWrapper<Note> | ODataResponse<Note>>('/odata/Notes/GetAll')
    
    // Проверяем формат с полем 'data' (кастомная обертка API)
    if (response.data && typeof response.data === 'object' && 'data' in response.data) {
      const wrapped = response.data as ApiResponseWrapper<Note>
      return wrapped.data || []
    }
    
    // Проверяем формат ответа OData (с полем 'value')
    if (response.data && typeof response.data === 'object' && 'value' in response.data) {
      return (response.data as ODataResponse<Note>).value || []
    }
    
    // Если это обычный массив
    if (Array.isArray(response.data)) {
      return response.data
    }
    
    // Неожиданный формат
    console.warn('Неожиданный формат ответа API:', response.data)
    return []
  },

  async getById(id: number): Promise<Note> {
    const response = await apiClient.get<Note | ApiResponseWrapper<Note>>(`/odata/Notes/GetById?noteId=${id}`)
    
    // Проверяем формат с полем 'data'
    if (response.data && typeof response.data === 'object' && 'data' in response.data) {
      const wrapped = response.data as ApiResponseWrapper<Note>
      return wrapped.data?.[0] as Note
    }
    
    return response.data as Note
  },

  async create(data: CreateNoteRequest): Promise<Note> {
    const response = await apiClient.post<Note | ApiResponseWrapper<Note>>('/odata/Notes/Create', data)
    
    // Проверяем формат с полем 'data'
    if (response.data && typeof response.data === 'object' && 'data' in response.data) {
      const wrapped = response.data as ApiResponseWrapper<Note>
      return wrapped.data?.[0] as Note
    }
    
    return response.data as Note
  },

  async update(data: UpdateNoteRequest): Promise<void> {
    await apiClient.put('/odata/Notes/Update', data)
  },

  async delete(id: number): Promise<void> {
    await apiClient.delete(`/odata/Notes/Delete?noteId=${id}`)
  },

  async exportToCsv(): Promise<Blob> {
    const response = await apiClient.get('/odata/Notes/ExportNotesToCsv', {
      responseType: 'blob'
    })
    return response.data
  }
}

// API методы для чата
export const chatApi = {
  async getUserRooms(userId: string): Promise<ChatRoom[]> {
    const response = await apiClient.get(`/api/Chat/GetUserRooms?userId=${userId}`)
    const data = response.data
    // Разворачиваем возможные форматы ответа
    if (data && typeof data === 'object') {
      // Наш глобальный фильтр: { data: [...] }
      if ('data' in data) {
        const wrapped = data as { data?: unknown }
        if (Array.isArray(wrapped.data)) return wrapped.data as ChatRoom[]
      }
      // OData: { value: [...] }
      if ('value' in data) {
        const odata = data as { value?: unknown }
        if (Array.isArray(odata.value)) return odata.value as ChatRoom[]
      }
    }
    // Прямой массив
    if (Array.isArray(data)) return data as ChatRoom[]
    console.warn('Неожиданный формат ответа для getUserRooms:', data)
    return []
  },

  async getRoomById(roomId: number): Promise<ChatRoom> {
    const response = await apiClient.get(`/api/Chat/GetRoomById?roomId=${roomId}`)
    const data = response.data
    if (data && typeof data === 'object') {
      if ('data' in data) {
        const wrapped = data as { data?: any }
        return (wrapped.data as ChatRoom) ?? ({} as ChatRoom)
      }
      if ('value' in data) {
        const odata = data as { value?: any }
        return (odata.value as ChatRoom) ?? ({} as ChatRoom)
      }
    }
    return data as ChatRoom
  },

  async getRoomMessages(roomId: number, skip = 0, take = 50): Promise<ChatMessage[]> {
    const response = await apiClient.get(
      `/api/Chat/GetRoomMessages?roomId=${roomId}&skip=${skip}&take=${take}`
    )
    const data = response.data
    if (data && typeof data === 'object') {
      if ('data' in data) {
        const wrapped = data as { data?: unknown }
        if (Array.isArray(wrapped.data)) return wrapped.data as ChatMessage[]
      }
      if ('value' in data) {
        const odata = data as { value?: unknown }
        if (Array.isArray(odata.value)) return odata.value as ChatMessage[]
      }
    }
    if (Array.isArray(data)) return data as ChatMessage[]
    console.warn('Неожиданный формат ответа для getRoomMessages:', data)
    return []
  },

  async createRoom(creatorUserId: string, data: CreateChatRoomRequest): Promise<ChatRoom> {
    const response = await apiClient.post(
      `/api/Chat/CreateRoom?creatorUserId=${creatorUserId}`,
      data
    )
    const resp = response.data
    if (resp && typeof resp === 'object' && 'data' in resp) {
      return (resp as { data?: any }).data as ChatRoom
    }
    return resp as ChatRoom
  },

  async sendMessage(
    senderId: string,
    senderName: string,
    data: SendMessageRequest
  ): Promise<ChatMessage> {
    const response = await apiClient.post(
      `/api/Chat/SendMessage?senderId=${senderId}&senderName=${senderName}`,
      data
    )
    const resp = response.data
    if (resp && typeof resp === 'object' && 'data' in resp) {
      return (resp as { data?: any }).data as ChatMessage
    }
    return resp as ChatMessage
  },

  async markMessagesAsRead(roomId: number): Promise<void> {
    await apiClient.post(`/api/Chat/MarkMessagesAsRead?roomId=${roomId}`)
  },

  async getOrCreatePrivateRoom(userId1: string, userId2: string): Promise<ChatRoom> {
    const response = await apiClient.post(
      `/api/Chat/GetOrCreatePrivateRoom?userId1=${userId1}&userId2=${userId2}`
    )
    const resp = response.data
    if (resp && typeof resp === 'object' && 'data' in resp) {
      return (resp as { data?: any }).data as ChatRoom
    }
    return resp as ChatRoom
  }
}

// API методы для друзей (IdentityServer)
export const friendsApi = {
  async sendRequest(requesterId: string, addresseeEmail: string): Promise<void> {
    await identityClient.post(`/api/Friends/SendRequest?requesterId=${encodeURIComponent(requesterId)}&addresseeEmail=${encodeURIComponent(addresseeEmail)}`)
  },

  async accept(friendshipId: number): Promise<void> {
    await identityClient.post(`/api/Friends/Accept?friendshipId=${friendshipId}`)
  },

  async reject(friendshipId: number): Promise<void> {
    await identityClient.post(`/api/Friends/Reject?friendshipId=${friendshipId}`)
  },

  async getFriends(userId: string): Promise<Array<{ id: string; email: string; fullName: string }>> {
    const response = await identityClient.get(`/api/Friends/GetFriends?userId=${encodeURIComponent(userId)}`)
    return Array.isArray(response.data) ? response.data : []
  },

  async getPending(userId: string): Promise<Array<{ id: number; requesterId: string; addresseeId: string; status: string }>> {
    const response = await identityClient.get(`/api/Friends/GetPending?userId=${encodeURIComponent(userId)}`)
    return Array.isArray(response.data) ? response.data : []
  }
}

export { apiClient, identityClient }

