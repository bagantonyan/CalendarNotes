import * as signalR from '@microsoft/signalr'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:7135'

class SignalRService {
  private connection: signalR.HubConnection | null = null
  private reconnectAttempts = 0
  private maxReconnectAttempts = 10
  private callbacks: Map<string, ((message: string) => void)[]> = new Map()

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(`${API_BASE_URL}/notificationHub`, {
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

    this.connection.on('ReceiveNotification', (message: string) => {
      console.log('✅ SignalR Notification:', message)
      const callbacks = this.callbacks.get('notification') || []
      callbacks.forEach((callback) => callback(message))
    })

    this.connection.onreconnecting((error) => {
      console.warn('⚠️ SignalR Reconnecting...', error)
      const callbacks = this.callbacks.get('reconnecting') || []
      callbacks.forEach((callback) => callback('reconnecting'))
    })

    this.connection.onreconnected((connectionId) => {
      console.log('✅ SignalR Reconnected!', connectionId)
      this.reconnectAttempts = 0
      const callbacks = this.callbacks.get('reconnected') || []
      callbacks.forEach((callback) => callback('connected'))
    })

    this.connection.onclose((error) => {
      console.error('❌ SignalR Connection Closed', error)
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
        console.warn('SignalR connection not initialized')
        return
      }

      if (this.connection.state === signalR.HubConnectionState.Connected) {
        console.log('SignalR already connected')
        return
      }

      // Если соединение находится в процессе подключения, ждем
      if (this.connection.state === signalR.HubConnectionState.Connecting) {
        console.log('SignalR connection already in progress')
        return
      }

      await this.connection.start()
      console.log('✅ SignalR Connected! Connection ID:', this.connection.connectionId)
      const callbacks = this.callbacks.get('connected') || []
      callbacks.forEach((callback) => {
        try {
          callback('connected')
        } catch (callbackError) {
          console.error('Error in connected callback:', callbackError)
        }
      })
    } catch (err) {
      console.error('❌ SignalR Connection Error:', err)
      const callbacks = this.callbacks.get('error') || []
      callbacks.forEach((callback) => {
        try {
          callback(err instanceof Error ? err.message : 'Connection error')
        } catch (callbackError) {
          console.error('Error in error callback:', callbackError)
        }
      })
      // Не пробрасываем ошибку дальше, чтобы не ломать приложение
      // throw err
    }
  }

  async stop(): Promise<void> {
    try {
      await this.connection?.stop()
      console.log('SignalR Disconnected')
    } catch (err) {
      console.error('Error stopping SignalR:', err)
    }
  }

  on(event: string, callback: (message: string) => void): void {
    if (!this.callbacks.has(event)) {
      this.callbacks.set(event, [])
    }
    this.callbacks.get(event)!.push(callback)
  }

  off(event: string, callback: (message: string) => void): void {
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
export const signalRService = new SignalRService()

