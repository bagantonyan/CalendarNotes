<template>
  <div class="chat-panel">
    <!-- Список комнат -->
    <div class="rooms-sidebar">
      <div class="sidebar-header">
        <h3>Чаты</h3>
        <span v-if="chatStore.totalUnreadCount > 0" class="badge">
          {{ chatStore.totalUnreadCount }}
        </span>
      </div>
      
      <div class="rooms-list">
        <div
          v-for="room in chatStore.sortedRooms"
          :key="room.id"
          class="room-item"
          :class="{ active: chatStore.currentRoom?.id === room.id }"
          @click="selectRoom(room)"
        >
          <div class="room-info">
            <div class="room-header">
              <h4 class="room-name">{{ getRoomName(room) }}</h4>
              <span v-if="room.unreadCount > 0" class="unread-badge">
                {{ room.unreadCount }}
              </span>
            </div>
            <p v-if="room.lastMessage" class="last-message">
              {{ room.lastMessage.senderName }}: {{ room.lastMessage.content }}
            </p>
          </div>
          <div class="room-time">
            {{ formatTime(room.modifiedDate) }}
          </div>
        </div>
      </div>
    </div>

    <!-- Окно чата -->
    <div class="chat-window">
      <div v-if="!chatStore.currentRoom" class="empty-state">
        <p>Выберите чат для начала общения</p>
      </div>

      <template v-else>
        <!-- Заголовок чата -->
        <div class="chat-header">
          <h3>{{ getRoomName(chatStore.currentRoom) }}</h3>
          <span class="participants-count">
            {{ chatStore.currentRoom.participantIds.length }} участников
          </span>
        </div>

        <!-- Сообщения -->
        <div class="messages-container" ref="messagesContainer">
          <div
            v-for="message in chatStore.sortedMessages"
            :key="message.id"
            class="message"
            :class="{ 
              'own-message': message.senderId === authStore.user?.id,
              'other-message': message.senderId !== authStore.user?.id
            }"
          >
            <div class="message-content">
              <div class="message-header">
                <span class="sender-name">{{ message.senderName }}</span>
                <span class="message-time">{{ formatTime(message.createdDate) }}</span>
              </div>
              <p class="message-text">{{ message.content }}</p>
            </div>
          </div>

          <!-- Индикатор печати -->
          <div
            v-if="typingUsers.length > 0"
            class="typing-indicator"
          >
            {{ typingUsers.join(', ') }} печатает...
          </div>
        </div>

        <!-- Поле ввода -->
        <div class="message-input-container">
          <input
            v-model="newMessage"
            type="text"
            placeholder="Введите сообщение..."
            class="message-input"
            @keyup.enter="sendMessage"
            @input="handleTyping"
          />
          <button
            @click="sendMessage"
            :disabled="!newMessage.trim()"
            class="send-button"
          >
            Отправить
          </button>
        </div>
      </template>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, nextTick, onUnmounted } from 'vue'
import { useChatStore } from '@/stores/chat'
import { useAuthStore } from '@/stores/auth'
import type { ChatRoom } from '@/types'

const chatStore = useChatStore()
const authStore = useAuthStore()

const newMessage = ref('')
const messagesContainer = ref<HTMLElement | null>(null)
const typingTimeout = ref<number | null>(null)

const typingUsers = computed(() => {
  if (!chatStore.currentRoom) return []
  return chatStore.getUsersTypingInRoom(chatStore.currentRoom.id)
})

// Получить название комнаты
function getRoomName(room: ChatRoom): string {
  if (room.name) return room.name
  
  // Для приватных чатов показываем имя другого участника
  if (!room.isGroupChat && authStore.user) {
    const otherUserId = room.participantIds.find(id => id !== authStore.user?.id)
    return otherUserId || 'Приватный чат'
  }
  
  return 'Чат'
}

// Форматировать время
function formatTime(dateString: string): string {
  const date = new Date(dateString)
  const now = new Date()
  const diffInMs = now.getTime() - date.getTime()
  const diffInHours = diffInMs / (1000 * 60 * 60)

  if (diffInHours < 24) {
    return date.toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' })
  } else {
    return date.toLocaleDateString('ru-RU', { day: '2-digit', month: '2-digit' })
  }
}

// Выбрать комнату
async function selectRoom(room: ChatRoom) {
  await chatStore.selectRoom(room)
  scrollToBottom()
}

// Отправить сообщение
async function sendMessage() {
  if (!newMessage.value.trim()) return

  try {
    await chatStore.sendMessage(newMessage.value)
    newMessage.value = ''
    
    // Уведомляем, что перестали печатать
    await chatStore.notifyStoppedTyping()
    
    // Прокручиваем к последнему сообщению
    await nextTick()
    scrollToBottom()
  } catch (error) {
    console.error('Ошибка отправки сообщения:', error)
  }
}

// Обработка печати
function handleTyping() {
  // Уведомляем, что печатаем
  chatStore.notifyTyping()

  // Сбрасываем таймер
  if (typingTimeout.value) {
    clearTimeout(typingTimeout.value)
  }

  // Через 3 секунды уведомляем, что перестали печатать
  typingTimeout.value = window.setTimeout(() => {
    chatStore.notifyStoppedTyping()
  }, 3000)
}

// Прокрутка к последнему сообщению
function scrollToBottom() {
  if (messagesContainer.value) {
    messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight
  }
}

// Следим за новыми сообщениями
watch(
  () => chatStore.sortedMessages,
  async () => {
    await nextTick()
    scrollToBottom()
  },
  { deep: true }
)

// Очистка таймера при размонтировании
onUnmounted(() => {
  if (typingTimeout.value) {
    clearTimeout(typingTimeout.value)
  }
})
</script>

<style scoped>
.chat-panel {
  display: flex;
  height: 600px;
  border: 1px solid #ddd;
  border-radius: 8px;
  overflow: hidden;
  background: white;
}

.rooms-sidebar {
  width: 300px;
  border-right: 1px solid #ddd;
  display: flex;
  flex-direction: column;
}

.sidebar-header {
  padding: 16px;
  border-bottom: 1px solid #ddd;
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: #f8f9fa;
}

.sidebar-header h3 {
  margin: 0;
  font-size: 18px;
}

.badge {
  background: #dc3545;
  color: white;
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 12px;
  font-weight: bold;
}

.rooms-list {
  flex: 1;
  overflow-y: auto;
}

.room-item {
  padding: 12px 16px;
  border-bottom: 1px solid #eee;
  cursor: pointer;
  transition: background 0.2s;
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

.room-item:hover {
  background: #f8f9fa;
}

.room-item.active {
  background: #e3f2fd;
}

.room-info {
  flex: 1;
  min-width: 0;
}

.room-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 4px;
}

.room-name {
  margin: 0;
  font-size: 14px;
  font-weight: 600;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.unread-badge {
  background: #007bff;
  color: white;
  padding: 2px 6px;
  border-radius: 10px;
  font-size: 11px;
  font-weight: bold;
  margin-left: 8px;
}

.last-message {
  margin: 0;
  font-size: 12px;
  color: #666;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.room-time {
  font-size: 11px;
  color: #999;
  margin-left: 8px;
  flex-shrink: 0;
}

.chat-window {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.empty-state {
  flex: 1;
  display: flex;
  justify-content: center;
  align-items: center;
  color: #999;
}

.chat-header {
  padding: 16px;
  border-bottom: 1px solid #ddd;
  background: #f8f9fa;
}

.chat-header h3 {
  margin: 0 0 4px 0;
  font-size: 18px;
}

.participants-count {
  font-size: 12px;
  color: #666;
}

.messages-container {
  flex: 1;
  overflow-y: auto;
  padding: 16px;
  background: #f8f9fa;
}

.message {
  margin-bottom: 16px;
  display: flex;
}

.own-message {
  justify-content: flex-end;
}

.other-message {
  justify-content: flex-start;
}

.message-content {
  max-width: 70%;
  padding: 8px 12px;
  border-radius: 8px;
}

.own-message .message-content {
  background: #007bff;
  color: white;
}

.other-message .message-content {
  background: white;
  border: 1px solid #ddd;
}

.message-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 4px;
  font-size: 11px;
}

.sender-name {
  font-weight: 600;
}

.own-message .sender-name {
  color: rgba(255, 255, 255, 0.9);
}

.other-message .sender-name {
  color: #333;
}

.message-time {
  margin-left: 8px;
  opacity: 0.7;
}

.message-text {
  margin: 0;
  font-size: 14px;
  word-wrap: break-word;
}

.typing-indicator {
  font-size: 12px;
  color: #666;
  font-style: italic;
  padding: 8px;
}

.message-input-container {
  display: flex;
  padding: 16px;
  border-top: 1px solid #ddd;
  background: white;
}

.message-input {
  flex: 1;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
  outline: none;
}

.message-input:focus {
  border-color: #007bff;
}

.send-button {
  margin-left: 8px;
  padding: 10px 20px;
  background: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: background 0.2s;
}

.send-button:hover:not(:disabled) {
  background: #0056b3;
}

.send-button:disabled {
  background: #ccc;
  cursor: not-allowed;
}
</style>

