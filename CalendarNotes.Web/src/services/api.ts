import axios, { type AxiosInstance } from 'axios'
import type {
  LoginRequest,
  RegisterRequest,
  AuthResponse,
  Note,
  CreateNoteRequest,
  UpdateNoteRequest
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

// API методы для заметок
export const notesApi = {
  async getAll(): Promise<Note[]> {
    const response = await apiClient.get<Note[]>('/odata/Notes/GetAll')
    return response.data
  },

  async getById(id: number): Promise<Note> {
    const response = await apiClient.get<Note>(`/odata/Notes/GetById?noteId=${id}`)
    return response.data
  },

  async create(data: CreateNoteRequest): Promise<Note> {
    const response = await apiClient.post<Note>('/odata/Notes/Create', data)
    return response.data
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

export { apiClient, identityClient }

