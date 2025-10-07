import { defineStore } from 'pinia'
import { ref } from 'vue'
import { signalRService } from '@/services/signalr'

export interface Notification {
  id: string
  message: string
  timestamp: Date
  read: boolean
}

export const useNotificationsStore = defineStore('notifications', () => {
  // State
  const notifications = ref<Notification[]>([])
  const connectionStatus = ref<'connected' | 'disconnected' | 'reconnecting'>('disconnected')
  const unreadCount = ref(0)

  // Генератор уникальных ID
  let notificationCounter = 0
  
  // Actions
  function addNotification(message: string): void {
    const notification: Notification = {
      id: `${Date.now()}-${notificationCounter++}`,
      message,
      timestamp: new Date(),
      read: false
    }
    notifications.value.unshift(notification)
    unreadCount.value++

    // Показываем браузерное уведомление
    showBrowserNotification(message)
  }

  function markAsRead(id: string): void {
    const notification = notifications.value.find((n) => n.id === id)
    if (notification && !notification.read) {
      notification.read = true
      unreadCount.value = Math.max(0, unreadCount.value - 1)
    }
  }

  function markAllAsRead(): void {
    notifications.value.forEach((n) => {
      n.read = true
    })
    unreadCount.value = 0
  }

  function clearAll(): void {
    notifications.value = []
    unreadCount.value = 0
  }

  function showBrowserNotification(message: string): void {
    if (!('Notification' in window)) {
      console.log('Browser does not support notifications.')
      return
    }

    if (Notification.permission === 'granted') {
      new Notification('CalendarNotes - Уведомление', {
        body: message,
        icon: '/vite.svg',
        badge: '/vite.svg'
      })
    } else if (Notification.permission !== 'denied') {
      Notification.requestPermission().then((permission) => {
        if (permission === 'granted') {
          new Notification('CalendarNotes - Уведомление', {
            body: message,
            icon: '/vite.svg',
            badge: '/vite.svg'
          })
        }
      })
    }
  }

  // Флаг для предотвращения повторной инициализации
  let isInitialized = false
  
  async function initSignalR(): Promise<void> {
    if (isInitialized) {
      console.log('SignalR уже инициализирован')
      return
    }
    
    try {
      // Подписываемся на уведомления перед подключением
      signalRService.on('notification', (message: string) => {
        try {
          addNotification(message)
        } catch (error) {
          console.error('Ошибка добавления уведомления:', error)
        }
      })

      signalRService.on('connected', () => {
        connectionStatus.value = 'connected'
      })

      signalRService.on('reconnecting', () => {
        connectionStatus.value = 'reconnecting'
      })

      signalRService.on('disconnected', () => {
        connectionStatus.value = 'disconnected'
      })
      
      // Подключаемся к SignalR
      await signalRService.start()
      connectionStatus.value = 'connected'
      isInitialized = true
    } catch (err) {
      console.error('Failed to initialize SignalR:', err)
      connectionStatus.value = 'disconnected'
    }
  }

  async function stopSignalR(): Promise<void> {
    try {
      await signalRService.stop()
      connectionStatus.value = 'disconnected'
      isInitialized = false
    } catch (error) {
      console.error('Ошибка остановки SignalR:', error)
    }
  }

  return {
    // State
    notifications,
    connectionStatus,
    unreadCount,
    // Actions
    addNotification,
    markAsRead,
    markAllAsRead,
    clearAll,
    initSignalR,
    stopSignalR
  }
})

