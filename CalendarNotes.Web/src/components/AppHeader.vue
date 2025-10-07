<template>
  <div class="app-header">
    <div class="logo" @click="router.push('/')">
      <n-icon size="32" color="#18a058">
        <CalendarIcon />
      </n-icon>
      <span class="logo-text">CalendarNotes</span>
    </div>

    <n-space align="center" :size="20">
      <!-- Индикатор подключения SignalR -->
      <n-tooltip>
        <template #trigger>
          <n-tag :type="connectionStatusType" size="small" round>
            <template #icon>
              <n-icon>
                <component :is="connectionStatusIcon" />
              </n-icon>
            </template>
            {{ connectionStatusText }}
          </n-tag>
        </template>
        SignalR: {{ notificationsStore.connectionStatus }}
      </n-tooltip>

      <!-- Меню навигации -->
      <n-menu
        v-if="authStore.isAuthenticated"
        mode="horizontal"
        :options="menuOptions"
        :value="activeKey"
        @update:value="handleMenuSelect"
      />

      <!-- Иконка уведомлений -->
      <n-badge
        v-if="authStore.isAuthenticated"
        :value="notificationsStore.unreadCount"
        :max="99"
        :show="notificationsStore.unreadCount > 0"
      >
        <n-button text @click="showNotifications = !showNotifications">
          <n-icon size="22">
            <NotificationsIcon />
          </n-icon>
        </n-button>
      </n-badge>

      <!-- Пользовательское меню -->
      <n-dropdown
        v-if="authStore.isAuthenticated"
        :options="userMenuOptions"
        @select="handleUserMenuSelect"
        trigger="click"
      >
        <n-button text>
          <n-icon size="22">
            <PersonIcon />
          </n-icon>
        </n-button>
      </n-dropdown>

      <!-- Кнопки входа/регистрации для неавторизованных -->
      <n-space v-else>
        <n-button @click="router.push('/login')">Вход</n-button>
        <n-button type="primary" @click="router.push('/register')">Регистрация</n-button>
      </n-space>
    </n-space>
  </div>
</template>

<script setup lang="ts">
import { computed, h, ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import {
  NSpace,
  NMenu,
  NButton,
  NIcon,
  NDropdown,
  NBadge,
  NTag,
  NTooltip,
  type MenuOption
} from 'naive-ui'
import {
  CalendarOutline as CalendarIcon,
  NotificationsOutline as NotificationsIcon,
  PersonOutline as PersonIcon,
  LogOutOutline as LogoutIcon,
  DocumentTextOutline as NotesIcon,
  HomeOutline as HomeIcon,
  CheckmarkCircle as ConnectedIcon,
  CloseCircle as DisconnectedIcon,
  Sync as ReconnectingIcon,
  ChatboxOutline as ChatIcon,
  PeopleOutline as FriendsIcon
} from '@vicons/ionicons5'
import { useAuthStore } from '@/stores/auth'
import { useNotificationsStore } from '@/stores/notifications'

const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()
const notificationsStore = useNotificationsStore()

const activeKey = computed(() => route.name as string)
const showNotifications = ref(false)

// Helper функция для создания иконок в меню
function renderIcon(icon: any) {
  return () => h(NIcon, null, { default: () => h(icon) })
}

const menuOptions = computed<MenuOption[]>(() => [
  {
    label: 'Главная',
    key: 'home',
    icon: renderIcon(HomeIcon)
  },
  {
    label: 'Заметки',
    key: 'notes',
    icon: renderIcon(NotesIcon)
  },
  {
    label: 'Друзья',
    key: 'friends',
    icon: renderIcon(FriendsIcon)
  },
  {
    label: 'Чат',
    key: 'chat',
    icon: renderIcon(ChatIcon)
  }
])

const userMenuOptions = computed(() => [
  {
    label: `${authStore.user?.fullName || 'Пользователь'}`,
    key: 'profile',
    disabled: true
  },
  {
    type: 'divider' as const,
    key: 'd1'
  },
  {
    label: 'Выйти',
    key: 'logout',
    icon: renderIcon(LogoutIcon)
  }
])

const connectionStatusType = computed(() => {
  switch (notificationsStore.connectionStatus) {
    case 'connected':
      return 'success'
    case 'reconnecting':
      return 'warning'
    case 'disconnected':
      return 'error'
    default:
      return 'default'
  }
})

const connectionStatusIcon = computed(() => {
  switch (notificationsStore.connectionStatus) {
    case 'connected':
      return ConnectedIcon
    case 'reconnecting':
      return ReconnectingIcon
    case 'disconnected':
      return DisconnectedIcon
    default:
      return DisconnectedIcon
  }
})

const connectionStatusText = computed(() => {
  switch (notificationsStore.connectionStatus) {
    case 'connected':
      return 'Подключено'
    case 'reconnecting':
      return 'Переподключение'
    case 'disconnected':
      return 'Отключено'
    default:
      return 'Нет связи'
  }
})

function handleMenuSelect(key: string) {
  router.push({ name: key })
}

function handleUserMenuSelect(key: string) {
  if (key === 'logout') {
    authStore.logout()
    router.push('/login')
  }
}
</script>

<style scoped>
.app-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 100%;
}

.logo {
  display: flex;
  align-items: center;
  gap: 12px;
  cursor: pointer;
  user-select: none;
}

.logo-text {
  font-size: 20px;
  font-weight: 600;
  color: #18a058;
}
</style>

