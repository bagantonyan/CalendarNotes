import * as signalR from '@microsoft/signalr'
import type { ChatMessage } from '@/types'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:7135'

class ChatSignalRService {
  private connection: signalR.HubConnection | null = null
  private reconnectAttempts = 0
  private maxReconnectAttempts = 10
  private callbacks: Map<string, ((data: any) => void)[]> = new Map()
  private currentUserId: string | null = null
  private currentUserName: string | null = null

  constructor() {
    this.initializeConnection()
  }

  private initializeConnection() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${API_BASE_URL}/chatHub`, {
        skipNegotiation: false,
        transport:
          signalR.HttpTransportType.WebSockets |
          signalR.HttpTransportType.ServerSentEvents |
          signalR.HttpTransportType.LongPolling
      })
      .configureLogging(signalR.LogLevel.Information)
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: (retryContext) => {
          if (retryContext.elapsedMilliseconds < 60000) {
            return Math.random() * 10000
          }
          return null
        }
      })
      .build()

    this.setupEventHandlers()
  }

  private setupEventHandlers() {
    if (!this.connection) return

    // Получение нового сообщения
    this.connection.on('ReceiveMessage', (message: ChatMessage) => {
      console.log('✅ Новое сообщение:', message)
      const callbacks = this.callbacks.get('message') || []
      callbacks.forEach((callback) => callback(message))
    })

    // Пользователь печатает
    this.connection.on('UserTyping', (roomId: number, userName: string) => {
      console.log(`⌨️ ${userName} печатает в комнате ${roomId}`)
      const callbacks = this.callbacks.get('userTyping') || []
      callbacks.forEach((callback) => callback({ roomId, userName }))
    })

    // Пользователь перестал печатать
    this.connection.on('UserStoppedTyping', (roomId: number, userName: string) => {
      const callbacks = this.callbacks.get('userStoppedTyping') || []
      callbacks.forEach((callback) => callback({ roomId, userName }))
    })

    // Сообщения прочитаны
    this.connection.on('MessagesRead', (roomId: number) => {
      console.log(`✅ Сообщения в комнате ${roomId} прочитаны`)
      const callbacks = this.callbacks.get('messagesRead') || []
      callbacks.forEach((callback) => callback(roomId))
    })

    // Регистрация пользователя подтверждена
    this.connection.on('UserRegistered', (userId: string) => {
      console.log('✅ Пользователь зарегистрирован в чате:', userId)
      const callbacks = this.callbacks.get('userRegistered') || []
      callbacks.forEach((callback) => callback(userId))
    })

    // Присоединение к комнате подтверждено
    this.connection.on('JoinedRoom', (roomId: number) => {
      console.log('✅ Присоединились к комнате:', roomId)
      const callbacks = this.callbacks.get('joinedRoom') || []
      callbacks.forEach((callback) => callback(roomId))
    })

    // Получение личного сообщения
    this.connection.on('ReceiveDirectMessage', (message: any) => {
      console.log('✅ Личное сообщение:', message)
      const callbacks = this.callbacks.get('directMessage') || []
      callbacks.forEach((callback) => callback(message))
    })

    // Ошибка
    this.connection.on('Error', (errorMessage: string) => {
      console.error('❌ Ошибка чата:', errorMessage)
      const callbacks = this.callbacks.get('error') || []
      callbacks.forEach((callback) => callback(errorMessage))
    })

    this.connection.onreconnecting((error) => {
      console.warn('⚠️ Chat SignalR Reconnecting...', error)
      const callbacks = this.callbacks.get('reconnecting') || []
      callbacks.forEach((callback) => callback('reconnecting'))
    })

    this.connection.onreconnected(async (connectionId) => {
      console.log('✅ Chat SignalR Reconnected!', connectionId)
      this.reconnectAttempts = 0
      
      // Повторная регистрация пользователя после переподключения
      if (this.currentUserId && this.currentUserName) {
        await this.registerUser(this.currentUserId, this.currentUserName)
      }
      
      const callbacks = this.callbacks.get('reconnected') || []
      callbacks.forEach((callback) => callback('connected'))
    })

    this.connection.onclose((error) => {
      console.error('❌ Chat SignalR Connection Closed', error)
      const callbacks = this.callbacks.get('closed') || []
      callbacks.forEach((callback) => callback('disconnected'))

      if (this.reconnectAttempts < this.maxReconnectAttempts) {
        this.reconnectAttempts++
        setTimeout(() => this.start(), 5000)
      }
    })
  }

  async start(): Promise<void> {
    try {
      if (!this.connection) {
        console.warn('Chat SignalR connection not initialized')
        return
      }

      if (this.connection.state === signalR.HubConnectionState.Connected) {
        console.log('Chat SignalR already connected')
        return
      }

      if (this.connection.state === signalR.HubConnectionState.Connecting) {
        console.log('Chat SignalR connection already in progress')
        return
      }

      await this.connection.start()
      console.log('✅ Chat SignalR Connected! Connection ID:', this.connection.connectionId)
      
      const callbacks = this.callbacks.get('connected') || []
      callbacks.forEach((callback) => callback('connected'))
    } catch (err) {
      console.error('❌ Chat SignalR Connection Error:', err)
      const callbacks = this.callbacks.get('error') || []
      callbacks.forEach((callback) =>
        callback(err instanceof Error ? err.message : 'Connection error')
      )
    }
  }

  async stop(): Promise<void> {
    try {
      await this.connection?.stop()
      this.currentUserId = null
      this.currentUserName = null
      console.log('Chat SignalR Disconnected')
    } catch (err) {
      console.error('Error stopping Chat SignalR:', err)
    }
  }

  // Регистрация пользователя в чате
  async registerUser(userId: string, userName: string): Promise<void> {
    if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
      console.error('Cannot register user: not connected')
      return
    }

    this.currentUserId = userId
    this.currentUserName = userName

    try {
      await this.connection.invoke('RegisterUser', userId, userName)
      console.log('✅ User registered in chat:', userId, userName)
    } catch (err) {
      console.error('Error registering user:', err)
    }
  }

  // Присоединиться к комнате
  async joinRoom(roomId: number): Promise<void> {
    if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
      console.error('Cannot join room: not connected')
      return
    }

    try {
      await this.connection.invoke('JoinRoom', roomId)
      console.log('✅ Joined room:', roomId)
    } catch (err) {
      console.error('Error joining room:', err)
    }
  }

  // Покинуть комнату
  async leaveRoom(roomId: number): Promise<void> {
    if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
      return
    }

    try {
      await this.connection.invoke('LeaveRoom', roomId)
      console.log('✅ Left room:', roomId)
    } catch (err) {
      console.error('Error leaving room:', err)
    }
  }

  // Отправить сообщение
  async sendMessage(
    senderId: string,
    senderName: string,
    roomId: number,
    content: string
  ): Promise<void> {
    if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
      console.error('Cannot send message: not connected')
      return
    }

    try {
      await this.connection.invoke('SendMessage', senderId, senderName, roomId, content)
      console.log('✅ Message sent')
    } catch (err) {
      console.error('Error sending message:', err)
      throw err
    }
  }

  // Уведомить, что пользователь печатает
  async userTyping(roomId: number, userName: string): Promise<void> {
    if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
      return
    }

    try {
      await this.connection.invoke('UserTyping', roomId, userName)
    } catch (err) {
      console.error('Error sending typing indicator:', err)
    }
  }

  // Уведомить, что пользователь перестал печатать
  async userStoppedTyping(roomId: number, userName: string): Promise<void> {
    if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
      return
    }

    try {
      await this.connection.invoke('UserStoppedTyping', roomId, userName)
    } catch (err) {
      console.error('Error sending stopped typing indicator:', err)
    }
  }

  // Отметить сообщения как прочитанные
  async markAsRead(roomId: number): Promise<void> {
    if (!this.connection || this.connection.state !== signalR.HubConnectionState.Connected) {
      return
    }

    try {
      await this.connection.invoke('MarkAsRead', roomId)
      console.log('✅ Messages marked as read')
    } catch (err) {
      console.error('Error marking messages as read:', err)
    }
  }

  // Подписка на события
  on(event: string, callback: (data: any) => void): void {
    if (!this.callbacks.has(event)) {
      this.callbacks.set(event, [])
    }
    this.callbacks.get(event)!.push(callback)
  }

  // Отписка от событий
  off(event: string, callback: (data: any) => void): void {
    const callbacks = this.callbacks.get(event) || []
    const index = callbacks.indexOf(callback)
    if (index > -1) {
      callbacks.splice(index, 1)
    }
  }

  getConnectionState(): string {
    return this.connection?.state || 'Disconnected'
  }

  isConnected(): boolean {
    return this.connection?.state === signalR.HubConnectionState.Connected
  }
}

// Singleton instance
export const chatSignalRService = new ChatSignalRService()

