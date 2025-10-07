<template>
  <n-config-provider :theme="isDarkTheme ? darkTheme : null" :locale="ruRU" :date-locale="dateRuRU">
    <n-notification-provider>
      <n-message-provider>
        <n-dialog-provider>
          <n-layout style="height: 100vh">
            <n-layout-header bordered style="height: 64px; padding: 0 24px">
              <AppHeader />
            </n-layout-header>
            
            <n-layout-content
              content-style="padding: 24px;"
              :native-scrollbar="false"
              style="height: calc(100vh - 64px)"
            >
              <router-view v-slot="{ Component }">
                <transition name="fade" mode="out-in">
                  <component v-if="Component" :is="Component" />
                </transition>
              </router-view>
            </n-layout-content>
          </n-layout>
          
          <!-- Компонент уведомлений -->
          <NotificationsPanel />
        </n-dialog-provider>
      </n-message-provider>
    </n-notification-provider>
  </n-config-provider>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, nextTick } from 'vue'
import {
  NConfigProvider,
  NLayout,
  NLayoutHeader,
  NLayoutContent,
  NNotificationProvider,
  NMessageProvider,
  NDialogProvider,
  darkTheme,
  ruRU,
  dateRuRU
} from 'naive-ui'
import AppHeader from './components/AppHeader.vue'
import NotificationsPanel from './components/NotificationsPanel.vue'
import { useNotificationsStore } from './stores/notifications'

const isDarkTheme = ref(false)
const isAppReady = ref(false)

onMounted(async () => {
  // Ждем следующего тика для завершения монтирования DOM
  await nextTick()
  
  try {
    // Инициализируем store и SignalR после монтирования компонента
    const notificationsStore = useNotificationsStore()
    await notificationsStore.initSignalR()
    
    // Запрашиваем разрешение на уведомления
    if ('Notification' in window && Notification.permission === 'default') {
      await Notification.requestPermission()
    }
  } catch (error) {
    console.error('Ошибка инициализации SignalR:', error)
  } finally {
    isAppReady.value = true
  }
})

onUnmounted(async () => {
  try {
    // Отключаемся от SignalR при размонтировании
    const notificationsStore = useNotificationsStore()
    await notificationsStore.stopSignalR()
  } catch (error) {
    console.error('Ошибка остановки SignalR:', error)
  }
})
</script>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

#app {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial,
    sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>

