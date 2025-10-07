import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { ChatRoom, ChatMessage } from '@/types'
import { chatApi } from '@/services/api'
import { chatSignalRService } from '@/services/chatSignalR'
import { useAuthStore } from './auth'

export const useChatStore = defineStore('chat', () => {
  // State
  const rooms = ref<ChatRoom[]>([])
  const currentRoom = ref<ChatRoom | null>(null)
  const messages = ref<ChatMessage[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)
  const isConnected = ref(false)
  const typingUsers = ref<Map<number, Set<string>>>(new Map())

  // Getters
  const sortedRooms = computed(() => {
    const list = Array.isArray(rooms.value) ? rooms.value : []
    return [...list].sort(
      (a, b) => new Date(b.modifiedDate).getTime() - new Date(a.modifiedDate).getTime()
    )
  })

  const sortedMessages = computed(() => {
    return [...messages.value].sort(
      (a, b) => new Date(a.createdDate).getTime() - new Date(b.createdDate).getTime()
    )
  })

  const totalUnreadCount = computed(() => {
    const list = Array.isArray(rooms.value) ? rooms.value : []
    return list.reduce((sum, room) => sum + (room?.unreadCount || 0), 0)
  })

  const getUsersTypingInRoom = computed(() => (roomId: number) => {
    return Array.from(typingUsers.value.get(roomId) || [])
  })

  // Actions
  async function initializeChat() {
    const authStore = useAuthStore()
    if (!authStore.user) {
      console.error('Пользователь не авторизован')
      return
    }

    try {
      // Подключаемся к SignalR
      await chatSignalRService.start()
      
      // Регистрируем пользователя
      await chatSignalRService.registerUser(authStore.user.id, authStore.user.fullName)
      
      isConnected.value = true

      // Подписываемся на события
      setupSignalRHandlers()

      // Загружаем комнаты пользователя
      await loadUserRooms()
    } catch (err) {
      console.error('Ошибка инициализации чата:', err)
      error.value = err instanceof Error ? err.message : 'Ошибка инициализации чата'
    }
  }

  function setupSignalRHandlers() {
    // Новое сообщение
    chatSignalRService.on('message', (message: ChatMessage) => {
      // Добавляем сообщение в список, если мы в этой комнате
      if (currentRoom.value?.id === message.chatRoomId) {
        messages.value.push(message)
      }

      // Обновляем информацию о комнате
      const room = rooms.value.find((r) => r.id === message.chatRoomId)
      if (room) {
        room.lastMessage = message
        room.modifiedDate = message.createdDate
        
        // Увеличиваем счетчик непрочитанных, если это не текущая комната
        if (currentRoom.value?.id !== message.chatRoomId) {
          room.unreadCount++
        }
      }
    })

    // Пользователь печатает
    chatSignalRService.on('userTyping', ({ roomId, userName }) => {
      if (!typingUsers.value.has(roomId)) {
        typingUsers.value.set(roomId, new Set())
      }
      typingUsers.value.get(roomId)!.add(userName)
    })

    // Пользователь перестал печатать
    chatSignalRService.on('userStoppedTyping', ({ roomId, userName }) => {
      if (typingUsers.value.has(roomId)) {
        typingUsers.value.get(roomId)!.delete(userName)
      }
    })

    // Сообщения прочитаны
    chatSignalRService.on('messagesRead', (roomId: number) => {
      const room = rooms.value.find((r) => r.id === roomId)
      if (room) {
        room.unreadCount = 0
      }
    })
  }

  async function loadUserRooms() {
    const authStore = useAuthStore()
    if (!authStore.user) return

    try {
      isLoading.value = true
      const result = await chatApi.getUserRooms(authStore.user.id)
      rooms.value = Array.isArray(result) ? result : []
    } catch (err) {
      console.error('Ошибка загрузки комнат:', err)
      error.value = err instanceof Error ? err.message : 'Ошибка загрузки комнат'
    } finally {
      isLoading.value = false
    }
  }

  async function selectRoom(room: ChatRoom) {
    try {
      isLoading.value = true
      currentRoom.value = room

      // Покидаем предыдущую комнату, если была
      if (currentRoom.value && currentRoom.value.id !== room.id) {
        await chatSignalRService.leaveRoom(currentRoom.value.id)
      }

      // Присоединяемся к новой комнате
      await chatSignalRService.joinRoom(room.id)

      // Загружаем сообщения
      messages.value = await chatApi.getRoomMessages(room.id)

      // Отмечаем сообщения как прочитанные
      await markRoomAsRead(room.id)
    } catch (err) {
      console.error('Ошибка выбора комнаты:', err)
      error.value = err instanceof Error ? err.message : 'Ошибка выбора комнаты'
    } finally {
      isLoading.value = false
    }
  }

  async function sendMessage(content: string) {
    const authStore = useAuthStore()
    if (!authStore.user || !currentRoom.value) return

    try {
      await chatSignalRService.sendMessage(
        authStore.user.id,
        authStore.user.fullName,
        currentRoom.value.id,
        content
      )
    } catch (err) {
      console.error('Ошибка отправки сообщения:', err)
      error.value = err instanceof Error ? err.message : 'Ошибка отправки сообщения'
      throw err
    }
  }

  async function markRoomAsRead(roomId: number) {
    try {
      await chatApi.markMessagesAsRead(roomId)
      await chatSignalRService.markAsRead(roomId)

      // Обновляем локальный счетчик
      const room = rooms.value.find((r) => r.id === roomId)
      if (room) {
        room.unreadCount = 0
      }
    } catch (err) {
      console.error('Ошибка отметки сообщений как прочитанных:', err)
    }
  }

  async function createPrivateRoom(userId2: string) {
    const authStore = useAuthStore()
    if (!authStore.user) return null

    try {
      isLoading.value = true
      const room = await chatApi.getOrCreatePrivateRoom(authStore.user.id, userId2)
      
      // Добавляем комнату в список, если её там нет
      if (!rooms.value.find((r) => r.id === room.id)) {
        rooms.value.push(room)
      }
      
      return room
    } catch (err) {
      console.error('Ошибка создания приватной комнаты:', err)
      error.value = err instanceof Error ? err.message : 'Ошибка создания комнаты'
      return null
    } finally {
      isLoading.value = false
    }
  }

  async function notifyTyping() {
    const authStore = useAuthStore()
    if (!authStore.user || !currentRoom.value) return

    await chatSignalRService.userTyping(currentRoom.value.id, authStore.user.fullName)
  }

  async function notifyStoppedTyping() {
    const authStore = useAuthStore()
    if (!authStore.user || !currentRoom.value) return

    await chatSignalRService.userStoppedTyping(currentRoom.value.id, authStore.user.fullName)
  }

  async function disconnect() {
    await chatSignalRService.stop()
    isConnected.value = false
    rooms.value = []
    currentRoom.value = null
    messages.value = []
  }

  return {
    // State
    rooms,
    currentRoom,
    messages,
    isLoading,
    error,
    isConnected,
    typingUsers,
    
    // Getters
    sortedRooms,
    sortedMessages,
    totalUnreadCount,
    getUsersTypingInRoom,
    
    // Actions
    initializeChat,
    loadUserRooms,
    selectRoom,
    sendMessage,
    markRoomAsRead,
    createPrivateRoom,
    notifyTyping,
    notifyStoppedTyping,
    disconnect
  }
})

