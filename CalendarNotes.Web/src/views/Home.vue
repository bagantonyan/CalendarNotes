<template>
  <n-space vertical :size="32" style="max-width: 1200px; margin: 0 auto">
    <n-card>
      <div style="text-align: center">
        <n-icon size="80" color="#18a058">
          <CalendarIcon />
        </n-icon>
        <h1 style="margin-top: 16px">Добро пожаловать в CalendarNotes</h1>
        <p style="font-size: 16px; color: #666; margin-top: 8px">
          Система управления заметками с уведомлениями
        </p>

        <n-space justify="center" style="margin-top: 24px">
          <n-button v-if="!authStore.isAuthenticated" type="primary" size="large" @click="router.push('/register')">
            Начать работу
          </n-button>
          <n-button v-else type="primary" size="large" @click="router.push('/notes')">
            Мои заметки
          </n-button>
        </n-space>
      </div>
    </n-card>

    <n-grid :cols="3" :x-gap="20">
      <n-grid-item>
        <n-card title="📝 Заметки" hoverable>
          Создавайте и управляйте своими заметками с удобным интерфейсом
        </n-card>
      </n-grid-item>
      <n-grid-item>
        <n-card title="⏰ Уведомления" hoverable>
          Получайте напоминания в указанное время через SignalR в реальном времени
        </n-card>
      </n-grid-item>
      <n-grid-item>
        <n-card title="🔒 Безопасность" hoverable>
          JWT аутентификация и IdentityServer для защиты ваших данных
        </n-card>
      </n-grid-item>
    </n-grid>
  </n-space>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { NSpace, NCard, NButton, NIcon, NGrid, NGridItem } from 'naive-ui'
import { CalendarOutline as CalendarIcon } from '@vicons/ionicons5'
import { useAuthStore } from '@/stores/auth'

const router = useRouter()
const authStore = useAuthStore()
</script>

