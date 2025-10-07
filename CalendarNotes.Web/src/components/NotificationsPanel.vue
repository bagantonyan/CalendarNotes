<template>
  <div class="notifications-container">
    <transition-group name="notification" tag="div" class="notifications-list">
      <div
        v-for="notification in visibleNotifications"
        :key="notification.id"
        class="notification-wrapper"
      >
        <n-card
          class="notification-card"
          size="small"
          :bordered="true"
          closable
          @close="handleClose(notification.id)"
        >
          <div class="notification-content">
            <n-icon size="24" color="#18a058">
              <NotificationsIcon />
            </n-icon>
            <div class="notification-text">
              <strong>📢 Уведомление</strong>
              <p>{{ notification.message }}</p>
              <small>{{ formatTime(notification.timestamp) }}</small>
            </div>
          </div>
        </n-card>
      </div>
    </transition-group>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { NCard, NIcon } from 'naive-ui'
import { NotificationsOutline as NotificationsIcon } from '@vicons/ionicons5'
import { useNotificationsStore } from '@/stores/notifications'

const notificationsStore = useNotificationsStore()

const visibleNotifications = computed(() => {
  // Безопасная фильтрация с проверкой на существование массива
  if (!notificationsStore.notifications || !Array.isArray(notificationsStore.notifications)) {
    return []
  }
  return notificationsStore.notifications
    .filter((n) => n && !n.read)
    .slice(0, 5)
})

function handleClose(id: string) {
  if (!id) return
  try {
    // Добавляем небольшую задержку для корректной работы анимации
    requestAnimationFrame(() => {
      notificationsStore.markAsRead(id)
    })
  } catch (error) {
    console.error('Ошибка при закрытии уведомления:', error)
  }
}

function formatTime(date: Date): string {
  try {
    const now = new Date()
    const notificationDate = new Date(date)
    
    // Проверка на валидную дату
    if (isNaN(notificationDate.getTime())) {
      return 'недавно'
    }
    
    const diff = now.getTime() - notificationDate.getTime()
    const seconds = Math.floor(diff / 1000)
    const minutes = Math.floor(seconds / 60)
    const hours = Math.floor(minutes / 60)

    if (seconds < 60) {
      return 'только что'
    } else if (minutes < 60) {
      return `${minutes} мин назад`
    } else if (hours < 24) {
      return `${hours} ч назад`
    } else {
      return notificationDate.toLocaleString('ru-RU')
    }
  } catch (error) {
    console.error('Ошибка форматирования времени:', error)
    return 'недавно'
  }
}
</script>

<style scoped>
.notifications-container {
  position: fixed;
  top: 80px;
  right: 20px;
  z-index: 9999;
  max-width: 400px;
  pointer-events: none;
}

.notifications-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
  position: relative;
}

.notification-wrapper {
  width: 100%;
  pointer-events: auto;
}

.notification-card {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  width: 100%;
}

.notification-content {
  display: flex;
  gap: 12px;
  align-items: flex-start;
}

.notification-text {
  flex: 1;
}

.notification-text p {
  margin: 4px 0;
  font-size: 14px;
}

.notification-text small {
  color: #999;
  font-size: 12px;
}

/* Анимации для transition-group */
.notification-enter-active {
  transition: all 0.4s ease-out;
}

.notification-leave-active {
  transition: all 0.4s ease-in;
  position: absolute;
  right: 0;
  width: 400px;
}

.notification-enter-from {
  transform: translateX(120%);
  opacity: 0;
}

.notification-leave-to {
  transform: translateX(120%);
  opacity: 0;
}

.notification-move {
  transition: transform 0.4s ease;
}
</style>

