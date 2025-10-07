import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { LoginRequest, RegisterRequest, AuthResponse, User } from '@/types'
import { authApi } from '@/services/api'

export const useAuthStore = defineStore('auth', () => {
  // State
  const token = ref<string | null>(localStorage.getItem('auth_token'))
  
  // Безопасно парсим user из localStorage
  const getUserFromStorage = (): User | null => {
    try {
      const userStr = localStorage.getItem('user')
      return userStr ? JSON.parse(userStr) : null
    } catch (error) {
      console.error('Error parsing user from localStorage:', error)
      localStorage.removeItem('user')
      return null
    }
  }
  
  const user = ref<User | null>(getUserFromStorage())
  const loading = ref(false)
  const error = ref<string | null>(null)

  // Getters
  const isAuthenticated = computed(() => !!token.value && !!user.value)

  // Actions
  async function login(credentials: LoginRequest): Promise<void> {
    try {
      loading.value = true
      error.value = null

      const response: AuthResponse = await authApi.login(credentials)

      // Сохраняем токен и пользователя
      token.value = response.token
      user.value = {
        id: response.userId,
        email: response.email,
        fullName: response.fullName
      }

      localStorage.setItem('auth_token', response.token)
      localStorage.setItem('user', JSON.stringify(user.value))
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Ошибка входа'
      throw err
    } finally {
      loading.value = false
    }
  }

  async function register(data: RegisterRequest): Promise<void> {
    try {
      loading.value = true
      error.value = null

      await authApi.register(data)

      // После успешной регистрации выполняем вход
      await login({ email: data.email, password: data.password })
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Ошибка регистрации'
      throw err
    } finally {
      loading.value = false
    }
  }

  function logout(): void {
    token.value = null
    user.value = null
    localStorage.removeItem('auth_token')
    localStorage.removeItem('user')
  }

  function clearError(): void {
    error.value = null
  }

  return {
    // State
    token,
    user,
    loading,
    error,
    // Getters
    isAuthenticated,
    // Actions
    login,
    register,
    logout,
    clearError
  }
})

