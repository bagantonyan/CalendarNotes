<template>
  <div class="notifications-container">
    <transition-group name="notification">
      <n-card
        v-for="notification in visibleNotifications"
        :key="notification.id"
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
  return notificationsStore.notifications.filter((n) => !n.read).slice(0, 5)
})

function handleClose(id: string) {
  notificationsStore.markAsRead(id)
}

function formatTime(date: Date): string {
  const now = new Date()
  const diff = now.getTime() - new Date(date).getTime()
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
    return new Date(date).toLocaleString('ru-RU')
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
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.notification-card {
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  animation: slideIn 0.3s ease-out;
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

@keyframes slideIn {
  from {
    transform: translateX(400px);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}

.notification-enter-active {
  animation: slideIn 0.3s ease-out;
}

.notification-leave-active {
  animation: slideOut 0.3s ease-in;
}

@keyframes slideOut {
  from {
    transform: translateX(0);
    opacity: 1;
  }
  to {
    transform: translateX(400px);
    opacity: 0;
  }
}
</style>

