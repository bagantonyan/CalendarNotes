<template>
  <div class="chat-page">
    <div class="container">
      <h1>Чат</h1>
      
      <div v-if="chatStore.isLoading" class="loading">
        Загрузка...
      </div>

      <div v-else-if="chatStore.error" class="error">
        {{ chatStore.error }}
      </div>

      <div v-else-if="!chatStore.isConnected" class="connection-status">
        <p>Подключение к чату...</p>
        <button @click="reconnect" class="btn btn-primary">
          Переподключиться
        </button>
      </div>

      <ChatPanel v-else />
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted } from 'vue'
import { useChatStore } from '@/stores/chat'
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'
import ChatPanel from '@/components/ChatPanel.vue'

const chatStore = useChatStore()
const authStore = useAuthStore()
const router = useRouter()

onMounted(async () => {
  // Проверяем авторизацию
  if (!authStore.isAuthenticated) {
    router.push('/login')
    return
  }

  // Инициализируем чат
  await chatStore.initializeChat()
})

onUnmounted(async () => {
  // Отключаемся при выходе со страницы
  await chatStore.disconnect()
})

async function reconnect() {
  await chatStore.initializeChat()
}
</script>

<style scoped>
.chat-page {
  padding: 20px;
  min-height: 100vh;
  background: #f5f5f5;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
}

h1 {
  margin-bottom: 20px;
  color: #333;
}

.loading, .error, .connection-status {
  text-align: center;
  padding: 40px;
  background: white;
  border-radius: 8px;
}

.error {
  color: #dc3545;
}

.connection-status {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 16px;
}

.btn {
  padding: 10px 20px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: background 0.2s;
}

.btn-primary {
  background: #007bff;
  color: white;
}

.btn-primary:hover {
  background: #0056b3;
}
</style>

