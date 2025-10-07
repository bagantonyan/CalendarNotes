// Типы для аутентификации
export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  email: string
  password: string
  firstName: string
  lastName: string
}

export interface AuthResponse {
  token: string
  email: string
  userId: string
  fullName: string
}

export interface User {
  id: string
  email: string
  fullName: string
}

// Типы для заметок
export interface Note {
  id: number
  title: string
  text: string
  notificationTime: string
  isNotified: boolean
  createdDate?: string
  modifiedDate?: string
}

export interface CreateNoteRequest {
  title: string
  text: string
  notificationTime: string
}

export interface UpdateNoteRequest {
  id: number
  title: string
  text: string
  notificationTime: string
}

// Типы для API ответов
export interface ApiResponse<T> {
  data: T
  success: boolean
  message?: string
}

