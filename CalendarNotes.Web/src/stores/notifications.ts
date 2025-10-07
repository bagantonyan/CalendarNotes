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

  // Actions
  function addNotification(message: string): void {
    const notification: Notification = {
      id: Date.now().toString(),
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

  async function initSignalR(): Promise<void> {
    try {
      // Подключаемся к SignalR
      await signalRService.start()
      connectionStatus.value = 'connected'

      // Подписываемся на уведомления
      signalRService.on('notification', (message: string) => {
        addNotification(message)
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
    } catch (err) {
      console.error('Failed to initialize SignalR:', err)
      connectionStatus.value = 'disconnected'
    }
  }

  async function stopSignalR(): Promise<void> {
    await signalRService.stop()
    connectionStatus.value = 'disconnected'
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

